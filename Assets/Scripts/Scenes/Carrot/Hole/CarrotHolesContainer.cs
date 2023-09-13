using System.Linq;

namespace Carrot.Spawners
{
    public class CarrotHolesContainer : HolesContainer<CarrotHole>
    {
        // check whether all holes are filled with water, if so, invoke [IsFillWithWater()] in each hole
        public bool IsAllHolesFillWithWater()
            => _holesOnScene.All(hole => hole.IsFillWithWater());
        
        // start holes blinking anim
        public void HolesBlink(bool isBlink)
        {
            foreach (var hole in _holesOnScene)
            {
                hole.HoleRedBlink(isBlink);
            }
        }
    }
}