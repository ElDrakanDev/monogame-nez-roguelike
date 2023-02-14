namespace Roguelike.Entities
{
    public interface ILifeTimed
    {
        public float LifeTime { get; set; }
        protected void TickLifeTime();
        protected void OnLifeTimeEnd();
    }
}
