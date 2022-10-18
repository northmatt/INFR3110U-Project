#include "GameObject.h"

GameObject::GameObject() {
}

float GameObject::LoadFromFile(int j, const char* filename) {
	line.clear();
	std::ifstream myFile;
	myFile.open(filename);

	float value = 0.f;
	while (myFile >> value) {
		line.push_back(value);
	}

	myFile.close();

	return line[j];
}

int GameObject::GetLines(const char* filename) {
	line.clear();
	std::ifstream myFile;
	myFile.open(filename);

	int value = 0;
	std::string tempString;
	while (std::getline(myFile, tempString)) {
		++value;
	}

	myFile.close();

	return value;
}
