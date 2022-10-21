#include "Wrapper.h"
#include "GameObject.h"

GameObject gameObject;

PLUGIN_API void StartSaving(const char* fileName)
{
	gameObject.StartSaving(fileName);
}

PLUGIN_API void WritePosition(Vector3D position)
{
	gameObject.WritePosition(position);
}

PLUGIN_API void EndSaving()
{
	gameObject.EndSaving();
}

PLUGIN_API void ReadData(const char* fileName)
{
	gameObject.ReadData(fileName);
}

PLUGIN_API Vector3D GetNthPosition(int n)
{
	return gameObject.GetNthPosition(n);
}

PLUGIN_API int GetLength()
{
	return gameObject.GetLength();
}
