using System.Collections;
using System.Collections.Generic;
using Marsion;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SerializeTest
{
    [Test]
    public void CardAbilityConvertTest()
    {
        Card card = new Card(0);
        ExampleAbility ability = ScriptableObject.CreateInstance<ExampleAbility>();
        ability.Type = AbilityType.Play;
        ability.Description = "HI Hello";

        byte[] bytes = JsonSerializer.SerializeBytes(ability);
        ExampleAbility dAbility = JsonSerializer.DeserializeBytes<ExampleAbility>(bytes);

        Assert.AreEqual(ability.Description, dAbility.Description);
    }

    [Test]
    public void CardConvertTest()
    {
        // Arrange

        // Act

        // Assert
        Card card = new Card(0);
        string uid = card.UID;
        byte[] bytes = JsonSerializer.SerializeBytes(card);
        Card dCard = JsonSerializer.DeserializeBytes<Card>(bytes);

        Assert.AreEqual(uid, dCard.UID);
    }
}
