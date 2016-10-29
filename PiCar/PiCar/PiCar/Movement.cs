using Newtonsoft.Json;

namespace PiCar
{
    internal class Movement
    {
        public bool Forward { get; set; }
        public bool Reverse { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        internal Movement()
        {
            Forward = false;
            Reverse = false;
            Left = false;
            Right = false;
        }

        public override string ToString() => JsonConvert.SerializeObject(this);
    }
}
