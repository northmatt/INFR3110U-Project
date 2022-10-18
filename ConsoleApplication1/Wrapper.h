#pragma once
#ifndef _WRAPPER_
#define _WRAPPER_

#include "PluginSettings.h"

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

	PLUGIN_API float loadFromFile(int j, const char* filename);
	PLUGIN_API int GetLines(const char* filename);

#ifdef __cplusplus
}
#endif // __cplusplus

#endif // !_WRAPPER_