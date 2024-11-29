using System.Collections;
using System.Collections.Generic;
using Marsion;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ManagerTest
{
    [UnityTest]
    public IEnumerator DataManagerTest()
    {
        yield return null;

        Assert.AreEqual(true, true);
    }
}
