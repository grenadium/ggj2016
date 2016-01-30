//SIUtils.cs
//
//Copyright (c) 2015 blkcatman
//
using UnityEngine;

public class SIUtils {
	public static bool CheckTypes(System.Type type)  {

		//Do not change below codes.
		if(type.Equals(typeof(SphericalImageCam_Equi4))) return false;
		if(type.Equals(typeof(SphericalImageCam_Equi5))) return false;
		if(type.Equals(typeof(SphericalImageCam_Equi6))) return false;
		if(type.Equals(typeof(SphericalImageCam_Dome3))) return false;
		if(type.Equals(typeof(SphericalImageCam_Dome5))) return false;
		if(type.Equals(typeof(SICameraBase))) return false;
		if(type.Equals(typeof(SICameraCreator))) return false;
		if(type.Equals(typeof(SICustomCamera))) return false;
		if(type.Equals(typeof(SIRenderEvent))) return false;
		if(type.Equals(typeof(SIGUITexture))) return false;

		if(type.Equals(typeof(GUILayer))) return false;
		if(type.Equals(typeof(AudioListener))) return false;
		if(type.Equals(typeof(AudioReverbZone))) return false;
		if(type.Equals(typeof(AudioClip))) return false;
		if(type.Equals(typeof(AudioSource))) return false;
		if(type.Equals(typeof(Camera))) return false;
		if(type.Equals(typeof(Transform))) return false;

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
		//for unity 5.x
		//Add codes if you want to ignore some Components.
		if(type.Equals(typeof(FlareLayer))) return false;




#endif
#if UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
		//for unity 4.x
		//Add codes if you want to ignore some Components.
		if(type.Equals(typeof(Behaviour))) return false;




#endif

		return true;
	}
}