#include "GameObject.h"

void GameObject::StartSaving(const char* fileName)
{
	saveFile.open(fileName);
}

void GameObject::WritePosition(Vector3D position)
{
	saveFile << position.x << std::endl;
	saveFile << position.y << std::endl;
	saveFile << position.z << std::endl;
}

void GameObject::EndSaving()
{
	saveFile.close();
}

void GameObject::ReadData(const char* fileName)
{
	positions.clear();
	readFile.open(fileName);
	Vector3D value;
	while (readFile >> value.x)
	{
		readFile >> value.y;
		readFile >> value.z;
		positions.push_back(value);
	}
	readFile.close();
}

Vector3D GameObject::GetNthPosition(int n)
{
	return positions[n];
}

int GameObject::GetLength()
{
	return positions.size();
}
