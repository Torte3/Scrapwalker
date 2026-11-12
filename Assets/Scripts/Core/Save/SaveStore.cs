using System;
using System.IO;
using System.Linq;
using Scrapwalker.Core.Framework;
using Scrapwalker.Core.Outpost.Upgrades;
using Scrapwalker.Core.Root;

namespace Scrapwalker.Core.Save
{
    public sealed class SaveStore
    {
        public const int AutoSlot = 0;
        public const int ManualSlots = 3;
        public const int SlotCount = ManualSlots + 1;

        private readonly GameDefinitions _definitions;
        private readonly Func<Definition, string> _nameOf;
        private readonly System.Action<string> _warn;
        private readonly System.Action<string> _error;

        public SaveStore(string root, GameDefinitions definitions, Func<Definition, string> nameOf, System.Action<string> warn, System.Action<string> error = null)
        {
            Root = root;
            _definitions = definitions;
            _nameOf = nameOf;
            _warn = warn;
            _error = error;
        }

        public string Root { get; }

        private string PathOf(int slot)
        {
            string name = slot == AutoSlot ? "scrapwalker.auto.save" : "scrapwalker." + slot + ".save";
            return Path.Combine(Root, name);
        }

        public bool Exists(int slot = AutoSlot)
        {
            return File.Exists(PathOf(slot));
        }

        public bool AnyExists()
        {
            return Enumerable.Range(AutoSlot, SlotCount).Any(Exists);
        }

        internal bool Write(SaveFile file, int slot)
        {
            try
            {
                string text = SaveWriter.Write(file);
                Directory.CreateDirectory(Root);
                string path = PathOf(slot);
                string temporary = path + ".tmp";
                File.WriteAllText(temporary, text);
                if (File.Exists(path))
                {
                    File.Replace(temporary, path, path + ".bak");
                }
                else
                {
                    File.Move(temporary, path);
                }

                return true;
            }
            catch (Exception e)
            {
                _warn?.Invoke("セーブに失敗しました: " + e.Message);
                return false;
            }
        }

        internal World Read(GameDefinitions definitions, int slot)
        {
            string path = PathOf(slot);
            World world = ReadFrom(path, definitions, out string failure);
            if (world != null)
            {
                return world;
            }

            World backup = ReadFrom(path + ".bak", definitions, out string backupFailure);
            if (backup != null)
            {
                if (failure != null)
                {
                    _warn?.Invoke(failure + "（控えから読みました）");
                }

                return backup;
            }

            if (failure != null)
            {
                _error?.Invoke(backupFailure != null ? failure + "\n" + backupFailure : failure);
            }

            return null;
        }

        public void Delete(int slot)
        {
            string path = PathOf(slot);
            Remove(path);
            Remove(path + ".bak");
        }

        public SaveSlot Describe(int slot)
        {
            string path = PathOf(slot);
            if (!File.Exists(path))
            {
                return SaveSlot.Empty(slot);
            }

            try
            {
                if (!SaveReader.TryParse(File.ReadAllText(path), out SaveFile file))
                {
                    return SaveSlot.Unreadable(slot);
                }

                WorldMemento world = file.World;
                return SaveSlot.Saved(slot, file.SavedAt.ToLocalTime(), world.Exploration.Furthest ?? string.Empty, world.Tick, world.Progress.Escaped,
                    OutpostUpgrades.TimeOfDayOf(world.Upgrades, _definitions));
            }
            catch (Exception e)
            {
                _warn?.Invoke("セーブの見出しを読めませんでした: " + path + " / " + e.Message);
                return SaveSlot.Unreadable(slot);
            }
        }

        public SaveSlot[] Describe()
        {
            var slots = new SaveSlot[SlotCount];
            for (int slot = 0; slot < slots.Length; slot++)
            {
                slots[slot] = Describe(slot);
            }

            return slots;
        }

        private void Remove(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e)
            {
                _warn?.Invoke("セーブの削除に失敗しました: " + path + " / " + e.Message);
            }
        }

        private World ReadFrom(string path, GameDefinitions definitions, out string failure)
        {
            failure = null;
            if (!File.Exists(path))
            {
                return null;
            }

            try
            {
                if (SaveReader.TryRead(File.ReadAllText(path), definitions, out World world, out LoadError error))
                {
                    return world;
                }

                failure = "セーブを読めませんでした: " + path + " / " + error.Kind + (error.Facility != null ? " " + _nameOf(error.Facility) : string.Empty) + (error.ID != null ? " " + error.ID : string.Empty);
                return null;
            }
            catch (Exception e)
            {
                failure = "セーブを読めませんでした: " + path + " / " + e.Message;
                return null;
            }
        }
    }
}
