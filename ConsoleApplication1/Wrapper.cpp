#include "Wrapper.h"
#include "GameObject.h"

GameObject gameObject;

PLUGIN_API float loadFromFile(int j, const char* filename) {
    return gameObject.LoadFromFile(j, filename);
}

PLUGIN_API int GetLines(const char* filename) {
    return gameObject.GetLines(filename);
}