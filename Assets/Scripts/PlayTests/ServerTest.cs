using System.Collections;
using Marsion;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ServerTest
{
    [UnityTest]
    public IEnumerator ServerTestWithEnumeratorPasses()
    {
        yield return new WaitForSeconds(5f);

        Assert.IsNotNull(Managers.Instance.Server);
    }
}
