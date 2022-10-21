#pragma once

#include <fstream>
#include <vector>

#include "PluginSettings.h"
#include "Vector3D.h"

class PLUGIN_API GameObject
{
public:
	void StartSaving(const char* fileName);
	void WritePosition(Vector3D position);
	void EndSaving();
	void ReadData(const char* fileName);
	Vector3D GetNthPosition(int n);
	int GetLength();

	std::ofstream saveFile;
	std::ifstream readFile;
	std::vector<Vector3D> positions;
};