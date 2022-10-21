#pragma once
#ifndef __WRAPPER__
#define __WRAPPER__

#include "PluginSettings.h"
#include "Vector3D.h"

#ifdef __cplusplus
extern "C"
{
#endif

	PLUGIN_API void StartSaving(const char* fileName);
	PLUGIN_API void WritePosition(Vector3D position);
	PLUGIN_API void EndSaving();

	PLUGIN_API void ReadData(const char* fileName);
	PLUGIN_API Vector3D GetNthPosition(int n);
	PLUGIN_API int GetLength();

#ifdef __cplusplus
}
#endif

#endif