using System;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehaviour : MonoBehaviour, IInteractable
{
	[TextArea(1, 2)]
	[SerializeField] private string OBS = "Choose a Type in inspector for now to set the resource data";
	public ResourceType type;
	[SerializeField] private ResourceDataSO _bunnyData; // This could later be a list of the different types of animals that this trap can catch.
	[SerializeField] private GameObject deadBrownBunnyGfx, deadWhiteBunnyGfx;
	[SerializeField] private SpriteRenderer _trapSpriteRenderer;
	[SerializeField] private Sprite _untriggeredTrapSprite, _triggeredTrapSprite;
	public TrapState state = TrapState.Untriggered;
	public event Action<GameObject> OnCollect;
	public event Action<GameObject, TrapBehaviour> PickupTrap;
	public int resourceAmount;
	public int listIndex;
	public float timeUntilCatch;
	public bool isPlaced = false;
	public EnergyCost costSize;

	private GameObject _gameObject;


	private void Start()
	{
		_gameObject = gameObject;
		resourceAmount = _bunnyData.resourceAmount;
		type = _bunnyData.type;
		costSize = _bunnyData.energyCostSize;

		Init();
	}

	private void Init()
	{
		//ResourceDataSO data;
		//int randomNumber = UnityEngine.Random.Range(0, 2); // This could later be the length of a list of mobs that can be caught, and here we raffle out an index on that list.
		//switch (randomNumber)
		//{
		//	case 0:
		//		data = _bunnyData;
		//		break;
		//	//case 1:
		//	//	break;
		//	default:
		//		data = _whiteBunnyData;
		//		break;
		//}

		state = TrapState.Untriggered;
		_trapSpriteRenderer.sprite = _untriggeredTrapSprite;
		deadBrownBunnyGfx.SetActive(false);
		deadWhiteBunnyGfx.SetActive(false);
	}

	private void OnEnable()
	{
		Init();
	}

	public void OnInteract()
	{
		if (state == TrapState.Triggered)
		{
			OnCollect?.Invoke(_gameObject);
		}
		TrapController.Instance.PickUpTrap(this);
		PickupTrap?.Invoke(_gameObject, this);
		_gameObject.SetActive(false);
	}

	public void OnDestruction() { }

	public void SetTrapToTriggered()
	{
		state = TrapState.Triggered;

		if (type == ResourceType.bunny)
		{
			int randomNumber = UnityEngine.Random.Range(0, 2);
			if (randomNumber == 0)
			{
				deadBrownBunnyGfx.SetActive(true);
			}
			else
			{
				deadWhiteBunnyGfx.SetActive(true);
			}
		}
		_trapSpriteRenderer.sprite = _triggeredTrapSprite;
	}
}

public enum TrapState { Untriggered, Triggered, }
