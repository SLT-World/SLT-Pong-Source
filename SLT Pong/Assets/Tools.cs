using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tools : MonoBehaviour
{
    public static void ChangeLayerInChildren(Transform _Transform, LayerMask _Layer)
    {
        _Transform.gameObject.layer = _Layer;
        foreach (Transform Child in _Transform)
            ChangeLayerInChildren(Child, _Layer);
    }

    public static float MPSToKMPH(float Param) =>
        Param * 3.6f;

    public static float KMPHToMPS(float Param) =>
        Param / 3.6f;

    public static Vector3 Direction(Vector3 A, Vector3 B) =>
        (A - B).normalized;

    public static Vector3 Heading(Vector3 A, Vector3 B) =>
        A - B;

    public static bool IsInCameraView(GameObject origin, GameObject toCheck)
    {
        try
        {
            if (!toCheck.GetComponentInChildren<Renderer>())
                return false;
            Vector3 pointOnScreen = Camera.current.WorldToScreenPoint(toCheck.GetComponentInChildren<Renderer>().bounds.center);

            if (pointOnScreen.z < 0)
                return false;
            //Debug.Log("Behind: " + toCheck.name);

            if ((pointOnScreen.x < 0) || (pointOnScreen.x > Screen.width) || (pointOnScreen.y < 0) || (pointOnScreen.y > Screen.height))
                return false;
            //Debug.Log("OutOfBounds: " + toCheck.name);

            RaycastHit _Hit;
            Vector3 Direction = (toCheck.transform.position - origin.transform.position).normalized;

            if (Physics.Linecast(Camera.current.transform.position, toCheck.GetComponentInChildren<Renderer>().bounds.center, out _Hit))
            {
                if (_Hit.transform.name != toCheck.name)
                    return false;
                //Debug.Log(toCheck.name + " occluded by " + hit.transform.name);
            }
            return true;
        }
        catch
        {
            try
            {
                Vector3 screenPoint = Camera.current.WorldToViewportPoint(toCheck.transform.position);
                return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
            }
            catch { return false; }
        }
    }

    public static int BoolToInt(bool Param) =>
        Param ? 1 : 0;

    public static bool IntToBool(int Param) =>
        Param == 1 ? true : false;

    public static string LocalIP() =>
        Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(item => item.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();

    public static T Clone<T>(T Source) => 
        JsonUtility.FromJson<T>(JsonUtility.ToJson(Source));

    public static List<Vector3> ByDistance(Vector3 Source, List<Vector3> _List) =>
        _List.OrderBy(item => Vector3.Distance(item, Source)).ToList();

    public static List<Transform> ByDistance(Vector3 Source, List<Transform> _List) =>
        _List.OrderBy(item => Vector3.Distance(item.position, Source)).ToList();

    public static IEnumerable<GameObject> FilterNull(IEnumerable<GameObject> Objects) =>
        Objects = Objects.Where(item => item);

    public static int RandomRange(int Minimum, int Maximum) =>
        Random.Range(Minimum, Maximum);

    public static float RandomRange(float Minimum, float Maximum) =>
        Random.Range(Minimum, Maximum);

    public static T[] FindObjectsOfTypeIncludingDisabled<T>()
    {
        var ActiveScene = SceneManager.GetActiveScene();
        var RootObjects = ActiveScene.GetRootGameObjects();
        var MatchObjects = new List<T>();

        foreach (var RootObject in RootObjects)
        {
            var Matches = RootObject.GetComponentsInChildren<T>(true);
            MatchObjects.AddRange(Matches);
        }

        return MatchObjects.ToArray();
    }

    public static Texture2D ReadableTexture2D(Texture2D _Texture)
    {
        RenderTexture _RenderTexture = RenderTexture.GetTemporary(_Texture.width, _Texture.height, 0, RenderTextureFormat.Default, RenderTextureReadWrite.Linear);
        Graphics.Blit(_Texture, _RenderTexture);

        RenderTexture PreviousRenderTexture = RenderTexture.active;
        RenderTexture.active = _RenderTexture;

        Texture2D ReadableTexture = new Texture2D(_Texture.width, _Texture.height);
        ReadableTexture.ReadPixels(new Rect(0, 0, _RenderTexture.width, _RenderTexture.height), 0, 0);
        ReadableTexture.Apply();

        RenderTexture.active = PreviousRenderTexture;
        RenderTexture.ReleaseTemporary(_RenderTexture);

        return ReadableTexture;
    }

    public static void TextureToImage(string _Path, Texture2D _Texture, ImageType _Type)
    {
        byte[] Bytes = null;
        switch (_Type)
        {
            case ImageType.PNG:
                Bytes = _Texture.EncodeToPNG();
                break;
            case ImageType.JPEG:
                Bytes = _Texture.EncodeToJPG();
                break;
            case ImageType.EXR:
                Bytes = _Texture.EncodeToEXR();
                break;
            case ImageType.TGA:
                Bytes = _Texture.EncodeToTGA();
                break;
        }
        File.WriteAllBytes(_Path, Bytes);
    }

    public void Save(string _Name, List<byte> Bytes)
    {
        BinaryFormatter Formatter = new BinaryFormatter();
        string _Path = Application.persistentDataPath + $"/{_Name}.save";
        FileStream _FileStream = new FileStream(_Path, FileMode.Create);

        Formatter.Serialize(_FileStream, Bytes);
        _FileStream.Close();
    }

    public List<byte> Load(string _Name)
    {
        List<byte> _Bytes = null;
        string _Path = Application.persistentDataPath + $"/{_Name}.save";
        if (File.Exists(_Path))
        {
            BinaryFormatter _BinaryFormatter = new BinaryFormatter();
            FileStream _FileStream = new FileStream(_Path, FileMode.Open);
            _Bytes = _BinaryFormatter.Deserialize(_FileStream) as List<byte>;
            _FileStream.Close();
        }
        return _Bytes;
    }

    public byte[] ToByteArray<T>(T _Object)
    {
        if (_Object == null)
            return null;
        BinaryFormatter Formatter = new BinaryFormatter();
        using (MemoryStream Stream = new MemoryStream())
        {
            Formatter.Serialize(Stream, _Object);
            return Stream.ToArray();
        }
    }

    public T FromByteArray<T>(byte[] Bytes)
    {
        if (Bytes == null)
            return default(T);
        BinaryFormatter Formatter = new BinaryFormatter();
        using (MemoryStream Stream = new MemoryStream(Bytes))
        {
            SurrogateSelector _SurrogateSelector = new SurrogateSelector();
            Vector3SerializationSurrogate Vector3Surrogate = new Vector3SerializationSurrogate();
            QuaternionSerializationSurrogate QuaternionSurrogate = new QuaternionSerializationSurrogate();

            _SurrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), Vector3Surrogate);
            _SurrogateSelector.AddSurrogate(typeof(Quaternion), new StreamingContext(StreamingContextStates.All), QuaternionSurrogate);
            Formatter.SurrogateSelector = _SurrogateSelector;
            object obj = Formatter.Deserialize(Stream);
            return (T)obj;
        }
    }

    /*public int KeyCodeToInt(KeyCode _KeyCode)
    {
        return
    }*/
}

public class Vector3SerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 v3 = (Vector3)obj;
        info.AddValue("x", v3.x);
        info.AddValue("y", v3.y);
        info.AddValue("z", v3.z);
    }
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 v3 = (Vector3)obj;
        v3.x = (float)info.GetValue("x", typeof(float));
        v3.y = (float)info.GetValue("y", typeof(float));
        v3.z = (float)info.GetValue("z", typeof(float));
        obj = v3;
        return obj;
    }
}
public class QuaternionSerializationSurrogate : ISerializationSurrogate
{
    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Quaternion _Quaternion = (Quaternion)obj;
        info.AddValue("x", _Quaternion.x);
        info.AddValue("y", _Quaternion.y);
        info.AddValue("z", _Quaternion.z);
        info.AddValue("w", _Quaternion.w);
    }
    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Quaternion _Quaternion = (Quaternion)obj;
        _Quaternion.x = (float)info.GetValue("x", typeof(float));
        _Quaternion.y = (float)info.GetValue("y", typeof(float));
        _Quaternion.z = (float)info.GetValue("z", typeof(float));
        _Quaternion.w = (float)info.GetValue("w", typeof(float));
        obj = _Quaternion;
        return obj;
    }
}

public enum FileType
{
    SLT,
    SAVE,
    BIN,
    RAW,
    ZIP,
}
public enum ImageType
{
    PNG,
    JPEG,
    GIF,
    SVG,
    BMP,
    EXR,
    TGA,
    TIFF,
}
public enum LabelType
{
    Default,
    Script,
    Advanced,
    Architecture,
    Prop,
    Weapon,
}