public interface IFoodState
{
	public IFoodState DoUpdateState(FoodBehaviour doThis, ResourceType type, Seasons season);

	public void StateBehaviour();

	public IFoodState ShouldChangeState(FoodBehaviour food, ResourceType type, Seasons season);
}