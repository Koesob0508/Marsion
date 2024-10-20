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
        // 1. 테스트용 카드 객체 생성
        ulong temp = 2;
        var originalCard = new Card(temp);

        // 2. Card 객체를 JSON 문자열로 직렬화
        string json = NetworkTool.JsonSerialize(originalCard);

        // 3. FastBufferWriter 초기화
        var bufferWriter = new FastBufferWriter(1024, Allocator.Temp, MarsNetwork.MessageSizeMax);

        // 5. JSON 문자열을 FastBufferWriter에 기록
        bufferWriter.WriteValueSafe(json);

        // 6. FastBufferReader로 변환
        var bufferReader = new FastBufferReader(bufferWriter, Allocator.Temp);

        // 7. FastBufferReader에서 JSON 문자열 읽기
        bufferReader.ReadValueSafe(out string readJson);

        // 8. JSON 문자열을 다시 Card 객체로 역직렬화
        var deserializedCard = NetworkTool.JsonDeserialize<Card>(readJson);

        // 9. 원래 카드 객체와 역직렬화된 카드 객체가 동일한지 검증
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

        // 10. 메모리 정리
        bufferWriter.Dispose();
        bufferReader.Dispose();
    }


    [Test]
    public void BufferMyDictionaryTest()
    {
        // 1. 테스트용 MyDictionary 객체 생성
        MyDictionary<string, Card> originalDictionary = new MyDictionary<string, Card>();

        // 2. Card 객체 생성 및 추가
        ulong playerId = 1;
        Card card1 = new Card(playerId);
        Card card2 = new Card(playerId);
        Card card3 = new Card(playerId);

        originalDictionary.Add(card1.UID, card1);
        originalDictionary.Add(card2.UID, card2);
        originalDictionary.Add(card3.UID, card3);

        // 3. MyDictionary 객체를 JSON 문자열로 직렬화
        string json = NetworkTool.JsonSerialize(originalDictionary);

        // 4. FastBufferWriter 초기화
        var bufferWriter = new FastBufferWriter(1024, Allocator.Temp, MarsNetwork.MessageSizeMax);

        // 5. JSON 문자열을 FastBufferWriter에 기록
        bufferWriter.WriteValueSafe(json);

        // 6. FastBufferReader로 변환
        var bufferReader = new FastBufferReader(bufferWriter, Allocator.Temp);

        // 7. FastBufferReader에서 JSON 문자열 읽기
        bufferReader.ReadValueSafe(out string readJson);

        // 8. JSON 문자열을 다시 MyDictionary 객체로 역직렬화
        MyDictionary<string, Card> deserializedDictionary = NetworkTool.JsonDeserialize<MyDictionary<string, Card>>(readJson);

        // 9. 원래의 MyDictionary 객체와 역직렬화된 객체가 동일한지 검증
        Assert.AreEqual(originalDictionary.Count, deserializedDictionary.Count); // 키-값 쌍의 개수 확인

        // 각 카드의 UID와 Name을 비교하여 검증
        foreach (var key in originalDictionary.Keys)
        {
            Assert.IsTrue(deserializedDictionary.TryGetValue(key, out var deserializedCard));
            originalDictionary.TryGetValue(key, out var originalCard);

            Assert.AreEqual(originalCard.UID, deserializedCard.UID);
            Assert.AreEqual(originalCard.PlayerID, deserializedCard.PlayerID);
        }

        // 10. 메모리 정리
        bufferWriter.Dispose();
        bufferReader.Dispose();
    }

    [Test]
    public void BufferPlayerTest()
    {
        // 1. 테스트용 Player 객체 생성
        Player originalPlayer = new Player(1);
        originalPlayer.Deck.Add(new Card(originalPlayer.ClientID));
        originalPlayer.Hand.Add(new Card(originalPlayer.ClientID));
        originalPlayer.Field.Add(new Card(originalPlayer.ClientID));

        // 2. Player 객체를 JSON 문자열로 직렬화
        string json = NetworkTool.JsonSerialize(originalPlayer);

        // 3. FastBufferWriter 초기화
        var bufferWriter = new FastBufferWriter(1024, Allocator.Temp, MarsNetwork.MessageSizeMax);

        // 4. JSON 문자열을 FastBufferWriter에 기록
        bufferWriter.WriteValueSafe(json);

        // 5. FastBufferReader로 변환
        var bufferReader = new FastBufferReader(bufferWriter, Allocator.Temp);

        // 6. FastBufferReader에서 JSON 문자열 읽기
        bufferReader.ReadValueSafe(out string readJson);

        // 7. JSON 문자열을 다시 Player 객체로 역직렬화
        Player deserializedPlayer = NetworkTool.JsonDeserialize<Player>(readJson);

        // 8. 원래 Player 객체와 역직렬화된 객체가 동일한지 검증
        Assert.AreEqual(originalPlayer.ClientID, deserializedPlayer.ClientID);
        Assert.AreEqual(originalPlayer.Card.UID, deserializedPlayer.Card.UID);
        Assert.AreEqual(originalPlayer.Deck.Count, deserializedPlayer.Deck.Count);
        Assert.AreEqual(originalPlayer.Hand.Count, deserializedPlayer.Hand.Count);
        Assert.AreEqual(originalPlayer.Field.Count, deserializedPlayer.Field.Count);

        // 각 카드의 UID를 비교하여 검증
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

        // 9. 메모리 정리
        bufferWriter.Dispose();
        bufferReader.Dispose();
    }

    [Test]
    public void BufferGameDataTest()
    {
        // 1. 테스트용 GameData 객체 생성
        GameData originalGameData = new GameData(2); // 두 플레이어를 가진 게임 데이터 생성

        // 각 플레이어에게 카드를 추가
        originalGameData.Players[0].Deck.Add(new Card(originalGameData.Players[0].ClientID));
        originalGameData.Players[0].Hand.Add(new Card(originalGameData.Players[0].ClientID));
        originalGameData.Players[0].Field.Add(new Card(originalGameData.Players[0].ClientID));

        originalGameData.Players[1].Deck.Add(new Card(originalGameData.Players[1].ClientID));
        originalGameData.Players[1].Hand.Add(new Card(originalGameData.Players[1].ClientID));
        originalGameData.Players[1].Field.Add(new Card(originalGameData.Players[1].ClientID));

        // 2. GameData 객체를 JSON 문자열로 직렬화
        string json = NetworkTool.JsonSerialize(originalGameData);

        // 3. FastBufferWriter 초기화
        var bufferWriter = new FastBufferWriter(1024, Allocator.Temp, MarsNetwork.MessageSizeMax);

        // 4. JSON 문자열을 FastBufferWriter에 기록
        bufferWriter.WriteValueSafe(json);

        // 5. FastBufferReader로 변환
        var bufferReader = new FastBufferReader(bufferWriter, Allocator.Temp);

        // 6. 현재 상황에 맞게 SerializedData에 FastBufferReader 넣기
        SerializedData sdata = new SerializedData(bufferReader);

        // 7. JSON 문자열을 다시 GameData 객체로 역직렬화
        GameData deserializedGameData = NetworkTool.JsonDeserialize<GameData>(sdata.GetString());

        // 8. 원래 GameData 객체와 역직렬화된 객체가 동일한지 검증
        Assert.AreEqual(originalGameData.TurnCount, deserializedGameData.TurnCount);
        Assert.AreEqual(originalGameData.Players.Length, deserializedGameData.Players.Length);

        // 각 플레이어의 카드 수를 비교하여 검증
        for (int i = 0; i < originalGameData.Players.Length; i++)
        {
            Assert.AreEqual(originalGameData.Players[i].ClientID, deserializedGameData.Players[i].ClientID);
            Assert.AreEqual(originalGameData.Players[i].Deck.Count, deserializedGameData.Players[i].Deck.Count);
            Assert.AreEqual(originalGameData.Players[i].Hand.Count, deserializedGameData.Players[i].Hand.Count);
            Assert.AreEqual(originalGameData.Players[i].Field.Count, deserializedGameData.Players[i].Field.Count);

            // 각 카드의 UID를 비교하여 검증
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

        // 9. 메모리 정리
        bufferWriter.Dispose();
        bufferReader.Dispose();
    }
}
