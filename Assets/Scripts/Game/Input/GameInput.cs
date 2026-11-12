using System;
using System.Collections.Generic;
using Scrapwalker.Core.Combat;
using Scrapwalker.Core.Outpost;
using Scrapwalker.Core.Outpost.Grid;
using Scrapwalker.Core.Root;
using Scrapwalker.Core.Sessions;
using Scrapwalker.Game.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scrapwalker.Game.Input
{
    public sealed class GameInput : IDisposable
    {
        private readonly Session _session;
        private readonly UIState _ui;
        private readonly IGridOnScreen _grid;
        private readonly ScrapwalkerInput _actions = new ScrapwalkerInput();
        private readonly List<RaycastResult> _uiHits = new List<RaycastResult>();

        public GameInput(Session session, UIState ui, IGridOnScreen grid)
        {
            _session = session;
            _ui = ui;
            _grid = grid;
            _actions.Game.Enable();
        }

        public Vector2 Pointer => _actions.Game.Point.ReadValue<Vector2>();

        public event Action LockedOn;

        public bool IsOverUI { get; private set; }

        public void Handle(bool isAccepting)
        {
            if (!isAccepting)
            {
                IsOverUI = false;
                return;
            }

            Vector2 screen = Pointer;
            IsOverUI = OverUI(screen);
            Cell? cursor = IsOverUI ? null : CellAt(screen);

            if (_actions.Game.Back.WasPressedThisFrame())
            {
                _ui.Back();
            }

            bool click = _actions.Game.Click.WasPressedThisFrame();
            if (_ui.Build.IsActive)
            {
                _ui.Build.Hover(cursor);
                if (click && cursor.HasValue)
                {
                    _ui.RunBuild(cursor.Value, _actions.Game.KeepPlacing.IsPressed());
                }

                return;
            }

            if (click && !IsOverUI && !TryLockOn(screen))
            {
                SelectAt(cursor);
            }
        }

        private bool TryLockOn(Vector2 screen)
        {
            int enemyID = _grid.EnemyAt(screen);
            if (enemyID == 0)
            {
                return false;
            }

            Enemy lockedOn = _session.World.Battle.LockedOn;
            if (lockedOn != null && lockedOn.ID == enemyID)
            {
                _session.Execute(new ReleaseLockOn());
            }
            else if (_session.Execute(new LockOn(enemyID)).IsOK)
            {
                LockedOn?.Invoke();
            }

            return true;
        }

        private void SelectAt(Cell? clicked)
        {
            if (!clicked.HasValue)
            {
                _ui.Select(null);
                return;
            }

            World world = _session.World;
            Cell cell = clicked.Value;
            Facility f = world.Facilities.At(cell);
            Facility moving = f == null ? world.Facilities.RelocatingInto(cell) : null;
            if (moving != null)
            {
                _ui.SelectAtRelocateTarget(moving);
                return;
            }

            _ui.Select(f);
        }

        public void Dispose()
        {
            _actions.Game.Disable();
            _actions.Dispose();
        }

        private Cell? CellAt(Vector2 screen)
        {
            return _grid.CellAt(screen, _session.World.Grid);
        }

        private bool OverUI(Vector2 screen)
        {
            if (EventSystem.current == null)
            {
                return false;
            }

            _uiHits.Clear();
            EventSystem.current.RaycastAll(new PointerEventData(EventSystem.current) { position = screen }, _uiHits);
            return _uiHits.Count > 0;
        }
    }
}
