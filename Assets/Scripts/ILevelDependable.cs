using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILevelDependable
{
    void OnLevelLoaded();

    void OnLevelCleanUp();
}
