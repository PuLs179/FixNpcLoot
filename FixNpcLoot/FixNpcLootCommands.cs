﻿using Torch.Commands;
using Torch.Commands.Permissions;
using VRage.Game.ModAPI;

namespace FixNpcLoot
{
    [Category("FixNpcLoot")]
    public class FixNpcLootCommands : CommandModule
    {

        public FixNpcLoot Plugin => (FixNpcLoot)Context.Plugin;

        [Command("test", "This is a Test Command.")]
        [Permission(MyPromoteLevel.Moderator)]
        public void Test()
        {
            Context.Respond("This is a Test from " + Context.Player);
        }

        [Command("testWithCommands", "This is a Test Command.")]
        [Permission(MyPromoteLevel.None)]
        public void TestWithArgs(string foo, string bar = null)
        {
            Context.Respond("This is a Test " + foo + ", " + bar);
        }
    }
}
