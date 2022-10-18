#pragma once
#ifndef _GAMEOBJECT_
#define _GAMEOBJECT_

#include <iostream>
#include <fstream>
#include <vector>
#include <string>

#include "PluginSettings.h"

class PLUGIN_API GameObject {
public:
	GameObject();

	float LoadFromFile(int j, const char* filename);
	int GetLines(const char* filename);

	std::vector<float> line;
};

#endif // !_GAMEOBJECT_

/*string save(string fileName, string toSave) {
	ofstream theFile;

	theFile.open(fileName);

	if (!theFile.is_open())
		return "Can't create file";

	theFile << toSave;

	theFile.close();
	return "Saved File";
}

string load(string fileName) {
	ifstream theFile;
	string cache;
	string currentLine;

	theFile.open(fileName);

	if (!theFile.is_open())
		return "Can't open file";

	while (getline(theFile, currentLine)) {
		cache += currentLine + "\n";
	}

	theFile.close();
	return cache;
}

int main() {
	string fileName;
	string input;

	cout << "Which file would like to load/save from: ";
	getline(cin, fileName);
	fileName += ".txt";

	cout << "Save or Load file: ";
	getline(cin, input);

	if (input == "load") {
		cout << load(fileName);
	}
	else if (input == "save") {
		cout << "What to save: ";
		getline(cin, input);
		cout << save(fileName, input);
	}

	cout << endl;

	return 0;
}*/