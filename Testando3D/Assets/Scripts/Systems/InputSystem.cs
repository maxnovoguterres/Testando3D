using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Assets.Scripts.Components;

namespace Assets.Scripts.Systems
{
    public class InputSystem : ComponentSystem
    {
        public struct Player
        {
            public InputComponent inputComponent;
        }

        protected override void OnUpdate()
        {

        }
    }
}
