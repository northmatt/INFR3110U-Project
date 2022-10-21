#include "GameObject.h"

void GameObject::StartSaving(const char* fileName)
{
	saveFile.open(fileName);
}

void GameObject::WritePosition(int type, Vector3D position)
{
	saveFile << type << std::endl;
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
	types.clear();
	positions.clear();
	readFile.open(fileName);
	int type;
	Vector3D value;
	while (readFile >> type)
	{
		readFile >> value.x;
		readFile >> value.y;
		readFile >> value.z;

		types.push_back(type);
		positions.push_back(value);
	}
	readFile.close();
}

Vector3D GameObject::GetNthPosition(int n)
{
	return positions[n];
}

int GameObject::GetNthType(int n)
{
	return types[n];
}

int GameObject::GetLength()
{
	return positions.size();
}
