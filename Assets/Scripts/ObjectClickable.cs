using UnityEngine;

public class ObjectClickable : MonoBehaviour
{
    public MenuIntroController menuIntro;

    //public string farmSize = "5ML";
    //public string scenario = "LightRainfall";
    //public string targetName = "IrrigationChannel";
    [TextArea] public string title = "Irrigation Delivery Channel";
    [TextArea] public string description = "carries water pumped by farmers from the reservoir.";
    public string unit = "mm";
    

    public void OnClicked()
    {
        Debug.Log("🟡 ObjectClickable.OnClicked() is activated！");

        menuIntro.ShowDashboard(menuIntro.GetCurrentFarmSize(),
                                menuIntro.GetCurrentScenario(),
                                menuIntro.GetCurrentTarget(),
                                title, 
                                description, 
                                unit);
        
    }
}
