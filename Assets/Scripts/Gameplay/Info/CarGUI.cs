using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class CarGUI : MonoBehaviour
{
    public static CarGUI Instance;
    public CarController _Car;
    [SerializeField] internal Text _SpeedText;

    private string __Display =          			
        	"{0:0} kmh \n" +
            "Gear: {1:0}/{2:0}\n" +
            "Revs {3:0%}\n" +
            "Throttle: {4:0%}\n";

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (_Car)
        {
            object[] args = new object[] { _Car.CurrentSpeed, _Car.GearNum + 1, _Car.NumberOfGear, _Car.Revs, _Car.AccelInput };
            _SpeedText.text = string.Format(__Display, args);
        }  
    }
}
