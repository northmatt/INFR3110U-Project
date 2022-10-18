#include "GameObject.h"

int main() {
    GameObject gameObject;

    std::cout << gameObject.GetLines("save.txt") << std::endl;
    std::cout << gameObject.LoadFromFile(1, "save.txt") << std::endl;
}
