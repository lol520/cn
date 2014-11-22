using System;
using LeagueSharp;
using LeagueSharp.Common;

namespace SAwareness
{
    internal class MoveToMouse
    {
        public MoveToMouse()
        {
            Game.OnGameUpdate += Game_OnGameUpdate;
        }

        ~MoveToMouse()
        {
            Game.OnGameUpdate -= Game_OnGameUpdate;
        }

        public bool IsActive()
        {
            return Menu.Misc.GetActive() && Menu.MoveToMouse.GetActive();
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            if (!IsActive() || !Menu.MoveToMouse.GetMenuItem("SAwarenessMoveToMouseKey").GetValue<KeyBind>().Active)
                return;

            ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }
    }
}