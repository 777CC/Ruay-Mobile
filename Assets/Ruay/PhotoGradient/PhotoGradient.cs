// Alan Zucconi
// www.alanzucconi.com
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

public class PhotoGradient : MonoBehaviour
{
    public MeshRenderer mashRen;
    public string ppp;
    void Start ()
    {
        SetColor();
    }
    private static byte[] Color32ArrayToByteArray(Color32[] colors)
    {
        if (colors == null || colors.Length == 0)
            return null;

        int lengthOfColor32 = Marshal.SizeOf(typeof(Color32));
        int length = lengthOfColor32 * colors.Length;
        byte[] bytes = new byte[length];

        GCHandle handle = default(GCHandle);
        try
        {
            handle = GCHandle.Alloc(colors, GCHandleType.Pinned);
            IntPtr ptr = handle.AddrOfPinnedObject();
            Marshal.Copy(ptr, bytes, 0, length);
        }
        finally
        {
            if (handle != default(GCHandle))
                handle.Free();
        }
        List<byte> data = new List<byte>();
        for (int i = 1; i <= bytes.Length; i++)
        {
            if(i % 4 != 0)
            {
                data.Add(bytes[i - 1]);
            }
            else
            {
                Debug.Log(i);
            }
        }
        return data.ToArray();
    }
    Color[] GetColorArrayFromByteArray(byte[] data)
    {
        Color[] color = new Color[data.Length / 3];
        for(int i = 0; i<color.Length;i++)
        {
            int index = i * 3;
            color[i] = new Color32(data[index], data[index + 1], data[index + 2], 0);
        }
        return color;
    }
    public void SetColor()
    {
        //Debug.Log(tex.GetPixels32().Length);
        //byte[] data = Color32ArrayToByteArray(tex.GetPixels32());
        //Debug.Log(data.Length);
        //Debug.Log(Convert.ToBase64String(data));



        //Color[] color = GetColorArrayFromByteArray(data);
        Color[] color = GetColorArrayFromByteArray(Convert.FromBase64String(ppp));

        // Get the elapsed time as a TimeSpan value.


        //tex.GetPixels32();
        //Texture2D photo = new Texture2D(5,4,TextureFormat.RGB24,true);
        //photo.LoadRawTextureData(System.Convert.FromBase64String(ppp));
        //Color[] color = photo.GetPixels();


        //16bit
        //byte[] data = tex.GetRawTextureData();
        //Debug.Log(data.Length);
        //Debug.Log(System.Convert.ToBase64String( tex.GetRawTextureData()));
        //Color[] color = tex.GetPixels();


        //4x3
        //Color[] color = new Color[15];
        //for (int i = 0; i < color.Length; i++)
        //{
        //    int col = Random.Range(0, 2);
        //    if(col == 0)
        //    {
        //        color[i] = Color.yellow;
        //    }
        //    else
        //    {
        //        color[i] = Color.blue;
        //    }
        //}
        //0.25f,0.5f,5,3
        //
        System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
        timer.Start();
        mashRen.material.SetFloat("dx", 0.2f);
        mashRen.material.SetFloat("dy", 0.3333333333333333f);
        //material.SetInt("SizeX", 16);
        //material.SetInt("SizeY", 16);
        mashRen.material.SetInt("SizeX", 6);
        mashRen.material.SetInt("SizeY", 4);
        mashRen.material.SetColorArray("_Points", color);
        mashRen.material.SetInt("_Points_Length", color.Length);
        timer.Stop();
        Debug.Log(timer.Elapsed.TotalSeconds);
    }

    void Update()
    {
        
        //for (int i = 0; i < positions.Length; i++)
        //{
        //    positions[i] += new Vector3(Random.Range(-0.1f,+0.1f), Random.Range(-0.1f, +0.1f)) * Time.deltaTime ;
        //    material.SetVector("_Points" + i.ToString(), positions[i]);
        //}

        
    }
}