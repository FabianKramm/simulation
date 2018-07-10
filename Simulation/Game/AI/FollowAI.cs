using Microsoft.Xna.Framework;
using Simulation.Game.Objects.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Simulation.Game.AI
{
    public class FollowAI: BaseAI
    {
        public FollowAI(MovingEntity movingEntity): base(movingEntity) { }

        public override void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}
