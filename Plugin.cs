using BepInEx;
using BepInEx.Logging;
using System.Collections;
using UnityEngine;

namespace SailwindVRFix
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private bool hintDisabled = false;
        private bool outlineFixed = false;
        private bool meshSaved = false;

        private GameObject CenterEyeAnchor;

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;
            Logger.LogInfo($"!Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!!!!!!!!!!!!!!!");

            StartCoroutine(DisableHintPanel());
            // StartCoroutine(FixOutlineCamera());
            // StartCoroutine(SaveMesh());
        }

        private IEnumerator DisableHintPanel()
        {
            while (!hintDisabled)
            {
                yield return new WaitForSeconds(5);
                DisableHintPanelImpl();
            }

        }

        private bool DisableHintPanelImpl()
        {
            if (hintDisabled) return true;
            Logger.LogInfo($"Attempt disabling hint text");

            CenterEyeAnchor = null;

            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>();
            foreach (GameObject obj in allObjects)
            {
                if (obj.CompareTag("MainCamera"))
                {
                    CenterEyeAnchor = obj;
                    Logger.LogInfo($"MainCamera found");
                    break;
                }
            }


            if(CenterEyeAnchor == null)
            {
                Logger.LogInfo($"MainCamera (CenterEyeAnchor) NOT found");
                return false;
            }

            Transform hintTextCube = CenterEyeAnchor.transform.Find("UI camera/hint text/Cube");

            if (hintTextCube == null)
            {
                Logger.LogInfo($"hint text NOT found");
                return false;
            }

            MeshRenderer renderer = hintTextCube.GetComponent<MeshRenderer>();
            if (hintTextCube == null)
            {
                Logger.LogInfo($"UI camera/hint text/Cube NOT found");
                return false;
            }


            renderer.enabled = false;
            hintDisabled = true;
            Logger.LogInfo($"UI camera/hint text/Cube disabled. Done!");


            return true;
        }

        private IEnumerator FixOutlineCamera()
        {
            while (!outlineFixed)
            {
                yield return new WaitForSeconds(5);
                FixOutlineCameraImpl();
            }

        }

        private bool FixOutlineCameraImpl()
        {

            if (CenterEyeAnchor == null) return false;
            Logger.LogInfo($"Attempt FixOutlineCamera");



            Transform VrChildCamera = CenterEyeAnchor.transform.Find("VrCameraOffset/VrChildCamera");
            if (VrChildCamera == null)
            {
                Logger.LogInfo($"VrCameraOffset/VrChildCamera NOT found");
                return false;
            }

            var comps = CenterEyeAnchor.GetComponents<Component>();

            Logger.LogInfo($"CenterEyeAnchor components count:{comps.Length}");

            for (int i = 0; i < comps.Length; i++)
            {
                Logger.LogInfo($"component at index:{i} type:{comps[i].GetType().Name}");

                if (comps[i].GetType().Name == "OutlineEffect")
                {
                    var compType = comps[i].GetType();
                    Destroy(comps[i]);
                    Logger.LogInfo($"found component OutlineEffect at index:{i}, create copy");
                    //VrChildCamera.gameObject.CopyComponent(comps[i]);
                    VrChildCamera.gameObject.AddComponent(compType);

                    //FieldInfo[] fields = comps[i].GetType().GetFields();
                    //foreach (FieldInfo field in fields)
                    //{
                    //    if (field.Name.Equals("enabled"))
                    //    {
                    //        field.SetValue(comps[i], false);
                    //    }
                    //}

                    outlineFixed = true;
                    break;
                }
            }



            return true;
        }


    }
}
