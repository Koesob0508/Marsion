using System.Collections;
using System.Collections.Generic;
using Marsion;
using Marsion.Logic;
using Marsion.Tool;
using NUnit.Framework;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.TestTools;

public class SerializeTest
{
    [Test]
    public void BufferCardTest()
    {
        // 1. �׽�Ʈ�� ī�� ��ü ����
        ulong temp = 2;
        var originalCard = new Card(temp);

        // 2. Card ��ü�� JSON ���ڿ��� ����ȭ
        string json = NetworkTool.JsonSerialize(originalCard);

        // 3. FastBufferWriter �ʱ�ȭ
        var bufferWriter = new FastBufferWriter(1024, Allocator.Temp, MarsNetwork.MessageSizeMax);

        // 5. JSON ���ڿ��� FastBufferWriter�� ���
        bufferWriter.WriteValueSafe(json);

        // 6. FastBufferReader�� ��ȯ
        var bufferReader = new FastBufferReader(bufferWriter, Allocator.Temp);

        // 7. FastBufferReader���� JSON ���ڿ� �б�
        bufferReader.ReadValueSafe(out string readJson);

        // 8. JSON ���ڿ��� �ٽ� Card ��ü�� ������ȭ
        var deserializedCard = NetworkTool.JsonDeserialize<Card>(readJson);

        // 9. ���� ī�� ��ü�� ������ȭ�� ī�� ��ü�� �������� ����
        Assert.AreEqual(originalCard.UID, deserializedCard.UID);
        Assert.AreEqual(originalCard.Name, deserializedCard.Name);
        Assert.AreEqual(originalCard.Grade, deserializedCard.Grade);
        Assert.AreEqual(originalCard.Mana, deserializedCard.Mana);
        Assert.AreEqual(originalCard.FullArtPath, deserializedCard.FullArtPath);
        Assert.AreEqual(originalCard.BoardArtPath, deserializedCard.BoardArtPath);
        Assert.AreEqual(originalCard.AbilityExplain, deserializedCard.AbilityExplain);
        Assert.AreEqual(originalCard.Attack, deserializedCard.Attack);
        Assert.AreEqual(originalCard.Health, deserializedCard.Health);
        Assert.AreEqual(originalCard.IsDead, deserializedCard.IsDead);

        // 10. �޸� ����
        bufferWriter.Dispose();
        bufferReader.Dispose();
    }


    [Test]
    public void BufferMyDictionaryTest()
    {
        // 1. �׽�Ʈ�� MyDictionary ��ü ����
        MyDictionary<string, Card> originalDictionary = new MyDictionary<string, Card>();

        // 2. Card ��ü ���� �� �߰�
        ulong playerId = 1;
        Card card1 = new Card(playerId);
        Card card2 = new Card(playerId);
        Card card3 = new Card(playerId);

        originalDictionary.Add(card1.UID, card1);
        originalDictionary.Add(card2.UID, card2);
        originalDictionary.Add(card3.UID, card3);

        // 3. MyDictionary ��ü�� JSON ���ڿ��� ����ȭ
        string json = NetworkTool.JsonSerialize(originalDictionary);

        // 4. FastBufferWriter �ʱ�ȭ
        var bufferWriter = new FastBufferWriter(1024, Allocator.Temp, MarsNetwork.MessageSizeMax);

        // 5. JSON ���ڿ��� FastBufferWriter�� ���
        bufferWriter.WriteValueSafe(json);

        // 6. FastBufferReader�� ��ȯ
        var bufferReader = new FastBufferReader(bufferWriter, Allocator.Temp);

        // 7. FastBufferReader���� JSON ���ڿ� �б�
        bufferReader.ReadValueSafe(out string readJson);

        // 8. JSON ���ڿ��� �ٽ� MyDictionary ��ü�� ������ȭ
        MyDictionary<string, Card> deserializedDictionary = NetworkTool.JsonDeserialize<MyDictionary<string, Card>>(readJson);

        // 9. ������ MyDictionary ��ü�� ������ȭ�� ��ü�� �������� ����
        Assert.AreEqual(originalDictionary.Count, deserializedDictionary.Count); // Ű-�� ���� ���� Ȯ��

        // �� ī���� UID�� Name�� ���Ͽ� ����
        foreach (var key in originalDictionary.Keys)
        {
            Assert.IsTrue(deserializedDictionary.TryGetValue(key, out var deserializedCard));
            originalDictionary.TryGetValue(key, out var originalCard);

            Assert.AreEqual(originalCard.UID, deserializedCard.UID);
            Assert.AreEqual(originalCard.PlayerID, deserializedCard.PlayerID);
        }

        // 10. �޸� ����
        bufferWriter.Dispose();
        bufferReader.Dispose();
    }

    [Test]
    public void BufferPlayerTest()
    {
        // 1. �׽�Ʈ�� Player ��ü ����
        Player originalPlayer = new Player(1);
        originalPlayer.Deck.Add(new Card(originalPlayer.ClientID));
        originalPlayer.Hand.Add(new Card(originalPlayer.ClientID));
        originalPlayer.Field.Add(new Card(originalPlayer.ClientID));

        // 2. Player ��ü�� JSON ���ڿ��� ����ȭ
        string json = NetworkTool.JsonSerialize(originalPlayer);

        // 3. FastBufferWriter �ʱ�ȭ
        var bufferWriter = new FastBufferWriter(1024, Allocator.Temp, MarsNetwork.MessageSizeMax);

        // 4. JSON ���ڿ��� FastBufferWriter�� ���
        bufferWriter.WriteValueSafe(json);

        // 5. FastBufferReader�� ��ȯ
        var bufferReader = new FastBufferReader(bufferWriter, Allocator.Temp);

        // 6. FastBufferReader���� JSON ���ڿ� �б�
        bufferReader.ReadValueSafe(out string readJson);

        // 7. JSON ���ڿ��� �ٽ� Player ��ü�� ������ȭ
        Player deserializedPlayer = NetworkTool.JsonDeserialize<Player>(readJson);

        // 8. ���� Player ��ü�� ������ȭ�� ��ü�� �������� ����
        Assert.AreEqual(originalPlayer.ClientID, deserializedPlayer.ClientID);
        Assert.AreEqual(originalPlayer.Card.UID, deserializedPlayer.Card.UID);
        Assert.AreEqual(originalPlayer.Deck.Count, deserializedPlayer.Deck.Count);
        Assert.AreEqual(originalPlayer.Hand.Count, deserializedPlayer.Hand.Count);
        Assert.AreEqual(originalPlayer.Field.Count, deserializedPlayer.Field.Count);

        // �� ī���� UID�� ���Ͽ� ����
        foreach (var card in originalPlayer.Deck)
        {
            Assert.IsTrue(deserializedPlayer.Deck.Exists(c => c.UID == card.UID));
        }

        foreach (var card in originalPlayer.Hand)
        {
            Assert.IsTrue(deserializedPlayer.Hand.Exists(c => c.UID == card.UID));
        }

        foreach (var card in originalPlayer.Field)
        {
            Assert.IsTrue(deserializedPlayer.Field.Exists(c => c.UID == card.UID));
        }

        // 9. �޸� ����
        bufferWriter.Dispose();
        bufferReader.Dispose();
    }

    [Test]
    public void BufferGameDataTest()
    {
        // 1. �׽�Ʈ�� GameData ��ü ����
        GameData originalGameData = new GameData(2); // �� �÷��̾ ���� ���� ������ ����

        // �� �÷��̾�� ī�带 �߰�
        originalGameData.Players[0].Deck.Add(new Card(originalGameData.Players[0].ClientID));
        originalGameData.Players[0].Hand.Add(new Card(originalGameData.Players[0].ClientID));
        originalGameData.Players[0].Field.Add(new Card(originalGameData.Players[0].ClientID));

        originalGameData.Players[1].Deck.Add(new Card(originalGameData.Players[1].ClientID));
        originalGameData.Players[1].Hand.Add(new Card(originalGameData.Players[1].ClientID));
        originalGameData.Players[1].Field.Add(new Card(originalGameData.Players[1].ClientID));

        // 2. GameData ��ü�� JSON ���ڿ��� ����ȭ
        string json = NetworkTool.JsonSerialize(originalGameData);

        // 3. FastBufferWriter �ʱ�ȭ
        var bufferWriter = new FastBufferWriter(1024, Allocator.Temp, MarsNetwork.MessageSizeMax);

        // 4. JSON ���ڿ��� FastBufferWriter�� ���
        bufferWriter.WriteValueSafe(json);

        // 5. FastBufferReader�� ��ȯ
        var bufferReader = new FastBufferReader(bufferWriter, Allocator.Temp);

        // 6. ���� ��Ȳ�� �°� SerializedData�� FastBufferReader �ֱ�
        SerializedData sdata = new SerializedData(bufferReader);

        // 7. JSON ���ڿ��� �ٽ� GameData ��ü�� ������ȭ
        GameData deserializedGameData = NetworkTool.JsonDeserialize<GameData>(sdata.GetString());

        // 8. ���� GameData ��ü�� ������ȭ�� ��ü�� �������� ����
        Assert.AreEqual(originalGameData.TurnCount, deserializedGameData.TurnCount);
        Assert.AreEqual(originalGameData.Players.Length, deserializedGameData.Players.Length);

        // �� �÷��̾��� ī�� ���� ���Ͽ� ����
        for (int i = 0; i < originalGameData.Players.Length; i++)
        {
            Assert.AreEqual(originalGameData.Players[i].ClientID, deserializedGameData.Players[i].ClientID);
            Assert.AreEqual(originalGameData.Players[i].Deck.Count, deserializedGameData.Players[i].Deck.Count);
            Assert.AreEqual(originalGameData.Players[i].Hand.Count, deserializedGameData.Players[i].Hand.Count);
            Assert.AreEqual(originalGameData.Players[i].Field.Count, deserializedGameData.Players[i].Field.Count);

            // �� ī���� UID�� ���Ͽ� ����
            foreach (var card in originalGameData.Players[i].Deck)
            {
                Assert.IsTrue(deserializedGameData.Players[i].Deck.Exists(c => c.UID == card.UID));
            }

            foreach (var card in originalGameData.Players[i].Hand)
            {
                Assert.IsTrue(deserializedGameData.Players[i].Hand.Exists(c => c.UID == card.UID));
            }

            foreach (var card in originalGameData.Players[i].Field)
            {
                Assert.IsTrue(deserializedGameData.Players[i].Field.Exists(c => c.UID == card.UID));
            }
        }

        // 9. �޸� ����
        bufferWriter.Dispose();
        bufferReader.Dispose();
    }
}
