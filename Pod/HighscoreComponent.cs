
/*
This code is released Open Source under the MIT license. Copyright 2008 Jon Watte, 
All Rights Reserved. You may use it free of charge for any purposes, provided that 
Jon Watte's copyright is reproduced in your use, and that you indemnify and hold 
harmless Jon Watte from any claim arising out of any use (or lack of use or lack of 
ability of use) you make of it. This software is provided as-is, without any 
warranty or guarantee, including any implicit guarantee of merchantability or fitness 
for any particular purpose. Use at your own risk!

For more information and updates, stop by my XNA programming area at
http://www.enchantedage.com/highscores
*/


using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Pod
{
  public class HighscoreComponent : GameComponent
  {
    NetworkSessionProperties identifier_;
    bool hasReadFile_;
    bool hasReadNetwork_;
    bool versionMismatchSeen_;
    bool saveSoon_;
    public bool userWantsToLoad_ = true;
    bool isJoin_;
    bool isCreate_;
    bool timeToDisconnect_;
    double sessionDuration_;
    float backoffTime_ = 1;
    PacketReader reader_ = new PacketReader();
    PacketWriter writer_ = new PacketWriter();
    const byte Version = 1;
    const int MaxAggregateCount = 120; //  top 50 network wide
    IAsyncResult async_;
    NetworkSession session_;
    Dictionary<string, Boolean> previousHosts_ = new Dictionary<string, Boolean>();
    Dictionary<string, List<Highscore>> userScores_ = new Dictionary<string, List<Highscore>>();
    Dictionary<NetworkGamer, List<Highscore>> toSend_ = new Dictionary<NetworkGamer, List<Highscore>>();
    List<Highscore> aggregateHighscores_ = new List<Highscore>();
    string titleName_;
    public static HighscoreComponent Global;

#if XBOX360
    public static string HostType = "Xbox360";
#else
    public static string HostType = "Win32";
#endif
    /// <summary>
    /// Create a new HighscoreComponent, which will keep track of your highscores.
    /// It will also use Xbox Live! networking to exchange scores with other users, 
    /// while the component is enabled. If you don't want this to happen in the 
    /// background while your game is running, disable the component at that time.
    /// </summary>
    /// <param name="g">The game.</param>
    /// <param name="identifier">How to identify the highscore sessions (can be null 
    /// if you don't want to use Live! yourself at other times).</param>
    /// <param name="titleName">The name of your game.</param>
    public HighscoreComponent(Game g, NetworkSessionProperties identifier, string titleName)
      : base(g)
    {
      identifier_ = identifier;
      titleName_ = titleName;
      Global = this;
    }
    
    /// <summary>
    /// Update the scores for the given player to reflect a new score result. 
    /// These scores may be pushed through the network in the future, and they 
    /// will be saved to disk/memory unit within the next few seconds.
    /// </summary>
    /// <param name="player">The player to record a new score for.</param>
    /// <param name="score">The score. This does not need to be a high score or top score.</param>
    /// <param name="message">A message to aggregate with the score -- for example, the max level 
    /// reached. Max 32 characters.</param>
    public void SetNewScore(PlayerIndex player, long score, string message)
    {
      string name = NameFromPlayer(player);
      List<Highscore> lhs;
      if (!userScores_.TryGetValue(name, out lhs))
      {
        lhs = new List<Highscore>();
        userScores_.Add(name, lhs);
      }
      Highscore hs = new Highscore();
      hs.Gamer = name;
      hs.Message = message;
      hs.Score = score;
      hs.When = DateTime.Now;
      hs.IsLocal = true;
      lhs.Add(hs);
      saveSoon_ = true;
    }
    
    public static string NameFromPlayer(PlayerIndex player)
    {
        // Altered from "* Not Signed In *";
      string name = "Guest";
      foreach (SignedInGamer sig in Gamer.SignedInGamers)
      {
        if (sig.PlayerIndex == player)
        {
          name = sig.Gamertag;
          break;
        }
      }
      return name;
    }
    
    public bool HasReadFile { get { return hasReadFile_; } }
    public bool HasReadNetwork { get { return hasReadNetwork_; } }

    /// <summary>
    /// Fill in a given array with highscores. Return the actual number of highscores 
    /// that were returned (which may be smaller). Returns highscores for everyone I've 
    /// ever seen.
    /// </summary>
    /// <param name="space">Where to put the highscores.</param>
    /// <returns>The number of highscores returned in the array.</returns>
    public int GetHighscores(Highscore[] space)
    {
      int n = Math.Min(space.Length, aggregateHighscores_.Count);
      for (int i = 0; i != space.Length; ++i)
      {
        space[i] = (i < n) ? aggregateHighscores_[i] : null;
      }
      return n;
    }

    /// <summary>
    /// Fill in a given array with highscores. Return the actual number of highscores 
    /// that were returned (which may be smaller). Returns highscores only for the 
    /// given player (or "Anonymous" if not signed in).
    /// </summary>
    /// <param name="space">Where to put the highscores.</param>
    /// <param name="player">The player to return highscores for.</param>
    /// <returns>The number of highscores returned in the array.</returns>
    public int GetHighscores(Highscore[] space, PlayerIndex player)
    {
      string name = NameFromPlayer(player);
      List<Highscore> lhs;
      if (!userScores_.TryGetValue(name, out lhs))
        lhs = new List<Highscore>();
      int n = Math.Min(space.Length, lhs.Count);
      for (int i = 0; i != space.Length; ++i)
      {
        space[i] = (i < n) ? lhs[i] : null;
      }
      return n;
    }

    /// <summary>
    /// Fill in a given array with highscores. Return the actual number of highscores 
    /// that were returned (which may be smaller). Returns highscores only for the 
    /// given player (or "Anonymous" if not signed in).
    /// </summary>
    /// <param name="space">Where to put the highscores.</param>
    /// <param name="player">The player to return highscores for.</param>
    /// <returns>The number of highscores returned in the array.</returns>
    public int GetHighscores(Highscore[] space, PlayerIndex player, string gameType)
    {
        string name = NameFromPlayer(player);
        List<Highscore> lhs;
        if (!userScores_.TryGetValue(name, out lhs))
            lhs = new List<Highscore>();

        int k = 0;
        for (int i = 0; i < lhs.Count; ++i)
        {
            if (lhs[i].Message.Equals(gameType))
            {
                space[k] = lhs[i];
                k++;
            }

            if (k >= space.Length)
                break;
        }

        return k;
    }
    
    /// <summary>
    /// Fill in a given array with highscores. Only get the highscores for the gametype we submitted.
    /// This is a custom method. Author: xSWOOPx
    /// </summary>
    /// <param name="space">This is where we put the highscores.</param>
    /// <param name="gameType">This is the string representation of the gametype for the highscores we are looking for.</param>
    /// <returns></returns>
    public int GetHighscores(Highscore[] space, string gameType)
    {
        int j = 0;
        for (int i = 0; i != aggregateHighscores_.Count; ++i)
        {
            if (aggregateHighscores_[i].Message.Equals(gameType))
            {
                space[j] = aggregateHighscores_[i];
                j++;
            }

            if (j >= space.Length)
            {
                break;
            }
        }
        return j;
    }

    public delegate bool FilterFunc<T>(T t);

    public static GameComponent Find<T>(IEnumerable<T> collection, FilterFunc<T> func) where T : class
    {
      foreach (T t in collection)
        if (func(t))
          return t as GameComponent;
      return null;
    }
    
    public override void Initialize()
    {
      base.Initialize();
      GamerServicesComponent gsc = Find<IGameComponent>(Game.Components, FilterGamerServicesComponent)
        as GamerServicesComponent;
      if (gsc == null)
        throw new InvalidOperationException("You must add the GamerServicesComponent to your component collection.");
    }
    
    private static bool FilterGamerServicesComponent(IGameComponent gc)
    {
      return gc is GamerServicesComponent;
    }

    protected override void Dispose(bool disposing)
    {
      StopImmediately();
      base.Dispose(disposing);
    }

    protected override void OnEnabledChanged(object sender, EventArgs args)
    {
      base.OnEnabledChanged(sender, args);
      if (this.Enabled == false)
        StopImmediately();
    }
    
    void StopImmediately()
    {
      if (this.async_ != null)
      {
        try
        {
          if (isCreate_)
          {
            NetworkSession.EndCreate(async_);
          }
          else if (isJoin_)
          {
            NetworkSession.EndJoin(async_);
          }
          else
          {
            NetworkSession.EndFind(async_);
          }
        }
        catch (System.Exception x)
        {
          System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HostType, x.Message));
        }
        async_ = null;
        isJoin_ = false;
        isCreate_ = false;
      }
      if (this.session_ != null)
      {
        try
        {
          //  abruptly terminate the session if you want to disable this component
          session_.Dispose();
        }
        catch (System.Exception x)
        {
          System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HostType, x.Message));
        }
        session_ = null;
      }
    }

    public void ClearHighscores()
    {
      clearOne_ = true;
      aggregateHighscores_ = new List<Highscore>();
      userScores_ = new Dictionary<string, List<Highscore>>();
      saveSoon_ = true;
    }
    
    public void PurgeHighscoresOlderThan(DateTime time)
    {
      bool removed = false;
      foreach (KeyValuePair<string, List<Highscore>> kvp in userScores_)
      {
        for (int i = 0, n = kvp.Value.Count; i != n; ++i)
        {
          Highscore hs = kvp.Value[i];
          if (hs.When < time)
          {
            kvp.Value.RemoveAt(i);
            --i;
            --n;
            removed = true;
          }
        }
      }
      if (removed)
        saveSoon_ = true;
    }

    IAsyncResult deviceAsync_;
    public StorageDevice storage_;
    bool clearOne_;

    public override void Update(GameTime gameTime)
    {
      if (session_ != null)
      {
        sessionDuration_ += gameTime.ElapsedRealTime.TotalSeconds;
        if (sessionDuration_ > 300)
        {
          timeToDisconnect_ = true;
        }
      }
      if (userWantsToLoad_ || saveSoon_)
      {
        if (storage_ != null && !storage_.IsConnected)
        {
          storage_ = null;
        }
        if (storage_ == null)
        {
          if (deviceAsync_ == null)
          {
            if (!Guide.IsVisible)
            {
              try
              {
                System.Diagnostics.Trace.WriteLine(String.Format("{0}: Beginning device selection.", HostType));
                deviceAsync_ = Guide.BeginShowStorageDeviceSelector(null, null);
              }
              catch (GuideAlreadyVisibleException gavx)
              {
                System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HostType, gavx.Message));
              }
            }
          }
          else if (deviceAsync_.IsCompleted)
          {
            try
            {
              storage_ = Guide.EndShowStorageDeviceSelector(deviceAsync_);
            }
            catch (System.Exception x)
            {
              System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HostType, x.Message));
            }
            deviceAsync_ = null;
            if (storage_ == null)
            {
              userWantsToLoad_ = false;
            }
          }
        }
        else
        {
          try
          {
            byte[] data = new byte[1000];
            GamePadState gps = GamePad.GetState(PlayerIndex.One);
            using (StorageContainer sc = storage_.OpenContainer(titleName_))
            {
              string path;
              path = System.IO.Path.Combine(sc.Path, "remote scores.dat");
              if (System.IO.File.Exists(path) && !clearOne_)
              {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                  while (!sr.EndOfStream)
                  {
                    string str = sr.ReadLine();
                    if (str == "")
                      break;
                    Highscore hs = new Highscore();
                    if (hs.Decode(str))
                    {
                      if (!userScores_.ContainsKey(hs.Gamer))
                        userScores_.Add(hs.Gamer, new List<Highscore>());
                      userScores_[hs.Gamer].Add(hs);
                    }
                  }
                }
              }
              path = System.IO.Path.Combine(sc.Path, "local scores.dat");
              if (System.IO.File.Exists(path) && !clearOne_)
              {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(path))
                {
                  while (!sr.EndOfStream)
                  {
                    string str = sr.ReadLine();
                    if (str == "")
                      break;
                    Highscore hs = new Highscore();
                    if (hs.Decode(str))
                    {
                      hs.IsLocal = true;
                      if (!userScores_.ContainsKey(hs.Gamer))
                        userScores_.Add(hs.Gamer, new List<Highscore>());
                      userScores_[hs.Gamer].Add(hs);
                    }
                  }
                }
              }
              //  now, sort out all scores
              AggregateScores();
              //  finally, write out the information we have
              path = System.IO.Path.Combine(sc.Path, "remote scores.dat");
              using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
              {
                foreach (Highscore hs in aggregateHighscores_)
                {
                  string str = hs.Encode();
                  if (str != null && str != "")
                    sw.WriteLine(str);
                }
                sw.WriteLine(); //  empty line ends the file
              }
              path = System.IO.Path.Combine(sc.Path, "local scores.dat");
              using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
              {
                foreach (KeyValuePair<string, List<Highscore>> kvp in userScores_)
                {
                  foreach (Highscore hs in kvp.Value)
                  {
                    if (hs.IsLocal)
                    {
                      string str = hs.Encode();
                      if (str != null && str != "")
                        sw.WriteLine(str);
                    }
                  }
                }
                sw.WriteLine(); //  empty line terminates file
              }
              userWantsToLoad_ = false;
              saveSoon_ = false;
              hasReadFile_ = true;
              clearOne_ = false;
            }
          }
          catch (System.Exception x)
          {
            System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HostType, x.Message));
            storage_ = null;
            userWantsToLoad_ = false;
            saveSoon_ = false;
          }
        }
      }
      base.Update(gameTime);
      if (backoffTime_ > 0)
      {
        backoffTime_ -= (float)gameTime.ElapsedRealTime.TotalSeconds;
        return;
      }
      try
      {
        if (this.Enabled == true && this.session_ == null && async_ == null)
        {
          backoffTime_ = 15;
          foreach (SignedInGamer sig in Gamer.SignedInGamers)
          {
            if (sig.Privileges.AllowOnlineSessions)
            {
              //  if I already got data from 10 people, then host myself instead
              if (previousHosts_.Count > 9)
              {
                //  OK, it's time for me to host
                System.Diagnostics.Trace.WriteLine(String.Format("{0}: Beginning PlayerMatch creation.", HostType));
                async_ = NetworkSession.BeginCreate(NetworkSessionType.PlayerMatch, 1, 31, 0,
                    identifier_, null, null);
                isCreate_ = true;
              }
              else 
              {
                System.Diagnostics.Trace.WriteLine(String.Format("{0}: Beginning PlayerMatch find.", HostType));
                async_ = NetworkSession.BeginFind(NetworkSessionType.PlayerMatch, 1, identifier_, null, null);
              }
              backoffTime_ = 0;
              break;
            }
          }
          if (async_ == null)
            System.Diagnostics.Trace.WriteLine(String.Format("{0}: No signed in gamer is allowed online sessions.", HostType));
        }
        if (async_ != null && async_.IsCompleted)
        {
          if (isJoin_)
          {
            isJoin_ = false;
            session_ = NetworkSession.EndJoin(async_);
            async_ = null;
            previousHosts_.Add(session_.Host.Gamertag, true);
            List<Highscore> lhs = new List<Highscore>();
            lhs.AddRange(aggregateHighscores_);
            toSend_.Add(session_.Host, lhs);
            session_.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(session__SessionEnded);
          }
          else if (isCreate_)
          {
            isCreate_ = false;
            session_ = NetworkSession.EndCreate(async_);
            async_ = null;
            session_.GamerJoined += new EventHandler<GamerJoinedEventArgs>(session__GamerJoined);
            session_.GamerLeft += new EventHandler<GamerLeftEventArgs>(session__GamerLeft);
            session_.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(session__SessionEnded);
          }
          else
          {
            backoffTime_ = 15;
            AvailableNetworkSessionCollection ansc = NetworkSession.EndFind(async_);
            async_ = null;
            foreach (AvailableNetworkSession ans in ansc)
            {
              if (!previousHosts_.ContainsKey(ans.HostGamertag))
              {
                //  connect to this guy
                isJoin_ = true;
                System.Diagnostics.Trace.WriteLine(String.Format("{0}: Beginning PlayerMatch join.", HostType));
                async_ = NetworkSession.BeginJoin(ans, null, null);
                backoffTime_ = 0;
              }
            }
            if (!isJoin_ && aggregateHighscores_.Count > 0)
            {
              //  OK, it's time for me to host
              System.Diagnostics.Trace.WriteLine(String.Format("{0}: Beginning PlayerMatch creation.", HostType));
              async_ = NetworkSession.BeginCreate(NetworkSessionType.PlayerMatch, 1, 31, 0, 
                  identifier_, null, null);
              isCreate_ = true;
              backoffTime_ = 0;
            }
          }
        }
      }
      catch (System.Exception x)
      {
        async_ = null;
        session_ = null;
        System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HostType, x.Message));
        backoffTime_ = 15;
      }
      if (session_ != null)
      {
        try
        {
          session_.Update();
          if (session_ == null) //  did it go away by an event from within update?
            return;
          foreach (LocalNetworkGamer lng in session_.LocalGamers)
          {
            NetworkGamer sender = null;
            bool gotdata = false;
            while (lng.IsDataAvailable)
            {
              gotdata = true;
              lng.ReceiveData(reader_, out sender);
              byte v = reader_.ReadByte();
              if (v != Version)
              {
                //  v == 0 just means "disconnect now"
                if (v != 0)
                {
                  System.Diagnostics.Trace.WriteLine(String.Format("{3}: Found protocol version {0} from {2}, I have {1}.",
                      v, Version, session_.Host.Gamertag, HostType));
                  versionMismatchSeen_ = true;
                }
                System.Diagnostics.Trace.WriteLine(String.Format("{0}: Disconnect packet from {1}", HostType, sender.Gamertag));
                timeToDisconnect_ = !session_.IsHost;
              }
              else
              {
                Highscore hs = new Highscore();
                if (hs.Read(reader_))
                {
                  hasReadNetwork_ = true;
                  if (!userScores_.ContainsKey(hs.Gamer))
                    userScores_.Add(hs.Gamer, new List<Highscore>());
                  userScores_[hs.Gamer].Add(hs);
                  System.Diagnostics.Trace.WriteLine(String.Format("{0}: Received highscore {1}",
                      HostType, hs.Encode()));
                }
              }
            }
            if (gotdata && session_ != null)
            {
              System.Diagnostics.Trace.WriteLine(String.Format("{1}: Got data from {0}", sender.Gamertag, HostType));
            }
          }
          System.Diagnostics.Debug.Assert(session_ != null);
          if (toSend_.Count > 0)
          {
            System.Diagnostics.Trace.WriteLine(String.Format("{0}: Sending data...", HostType));
            NetworkGamer toRemove = null;
            foreach (KeyValuePair<NetworkGamer, List<Highscore>> kvp in toSend_)
            {
              //  create the disconnect packet
              if (kvp.Value.Count == 0)
              {
                System.Diagnostics.Trace.WriteLine(String.Format("{1}: Writing bye-bye packet to {0}", kvp.Key.Gamertag, HostType));
                writer_.Write((byte)0);
                session_.LocalGamers[0].SendData(writer_, SendDataOptions.ReliableInOrder, kvp.Key);
                toRemove = kvp.Key;
              }
              else
              {
                System.Diagnostics.Trace.WriteLine(String.Format("{2}: Writing packet to {0}, {1} to go.", kvp.Key.Gamertag, kvp.Value.Count, HostType));
                writer_.Write((byte)Version);
                if (kvp.Value[kvp.Value.Count - 1].Write(writer_))
                  session_.LocalGamers[0].SendData(writer_, SendDataOptions.Reliable, kvp.Key);
                kvp.Value.RemoveAt(kvp.Value.Count - 1);
              }
            }
            if (toRemove != null)
            {
              System.Diagnostics.Trace.WriteLine(String.Format("{0}: Removing {1} from list of send destinations.",
                  HostType, toRemove.Gamertag));
              toSend_.Remove(toRemove);
            }
          }
          else if (timeToDisconnect_)
          {
            System.Diagnostics.Trace.WriteLine(String.Format("{0}: Time has come to say good-bye!", HostType));
            session__SessionEnded(null, null);
          }
        }
        catch (System.Exception x)
        {
          System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HostType, x.Message));
          session_ = null;
          //  effectively disable any more updates
          backoffTime_ = 1e30f;
        }
      }
    }

    void session__SessionEnded(object sender, NetworkSessionEndedEventArgs e)
    {
      System.Diagnostics.Trace.WriteLine(String.Format("{0}: SessionEnded", HostType));
      if (session_ != null)
      {
        sessionDuration_ = 0;
        timeToDisconnect_ = false;
        session_.Dispose();
        session_ = null;
        AggregateScores();
      }
      toSend_ = new Dictionary<NetworkGamer, List<Highscore>>();
      //  create a new session in 15 seconds
      backoffTime_ = 15;
    }

    void session__GamerLeft(object sender, GamerLeftEventArgs e)
    {
      System.Diagnostics.Trace.WriteLine(String.Format("{0}: GamerLeft: {1}", HostType, e.Gamer.Gamertag));
      if (toSend_.ContainsKey(e.Gamer))
        toSend_.Remove(e.Gamer);
      saveSoon_ = true;
    }

    void session__GamerJoined(object sender, GamerJoinedEventArgs e)
    {
      if (!e.Gamer.IsLocal)
      {
        foreach (LocalNetworkGamer lng in session_.LocalGamers)
        {
          lng.EnableSendVoice(e.Gamer, false);
        }
      }
      //  don't send to myself
      System.Diagnostics.Trace.WriteLine(String.Format("{0}: GamerJoined: {1}", HostType, e.Gamer.Gamertag));
      if (e.Gamer == session_.LocalGamers[0])
        return;
      List<Highscore> lhs = new List<Highscore>();
      lhs.InsertRange(0, aggregateHighscores_);
      toSend_.Add(e.Gamer, lhs);
    }

    void AggregateScores()
    {
      System.Diagnostics.Trace.WriteLine(String.Format("{0}: Calculating Aggregate Scores", HostType));
      aggregateHighscores_.Clear();
      foreach (KeyValuePair<string, List<Highscore>> kvp in userScores_)
      {
        if (kvp.Value.Count > 0)
        {
          //  make sure it's sorted
          kvp.Value.Sort();
          //  remove duplicates
          Highscore prev = kvp.Value[0];
          for (int i = 1, n = kvp.Value.Count; i != n; ++i)
          {
            Highscore hs = kvp.Value[i];
            if (hs.Gamer == prev.Gamer && hs.When == prev.When && hs.Score == prev.Score)
            {
              if (!hs.IsLocal)
                kvp.Value.RemoveAt(i);
              else
                kvp.Value.RemoveAt(i-1);
              --i;
              --n;
            }
            else
            {
              prev = hs;
            }
          }
          //  make sure each entry is not too big
            /*
          if (kvp.Value.Count > MaxAggregateCount)
          {
            kvp.Value.RemoveRange(MaxAggregateCount, kvp.Value.Count - MaxAggregateCount);
          }*/
        }
        foreach (Highscore hs in kvp.Value)
        {
          aggregateHighscores_.Add(hs);
        }
      }
      //  sort and prune the "remote" high score list
      if (aggregateHighscores_.Count > 0)
      {
        aggregateHighscores_.Sort();

        // Custom code to keep the top 20 highscores of each gametype
        List<Highscore> tempHighscores = new List<Highscore>();
        if (aggregateHighscores_.Count > MaxAggregateCount)
        {
            int arcadeCount = 0;
            int survivalCount = 0;
            int zonesCount = 0;
            int travelCount = 0;
            int thinkFastCount = 0;
            for (int i = 0; i < aggregateHighscores_.Count; i++)
            {
                Highscore hScore = aggregateHighscores_[i];
                if (hScore.Message.Equals("Arcade") && arcadeCount < 20)
                {
                    arcadeCount++;
                    tempHighscores.Add(hScore);
                }
                else if (hScore.Message.Equals("Survival") && survivalCount < 20)
                {
                    survivalCount++;
                    tempHighscores.Add(hScore);
                }
                else if (hScore.Message.Equals("Zones") && zonesCount < 20)
                {
                    zonesCount++;
                    tempHighscores.Add(hScore);
                }
                else if (hScore.Message.Equals("Travel") && travelCount < 20)
                {
                    travelCount++;
                    tempHighscores.Add(hScore);
                }
                else if (hScore.Message.EndsWith("ThinkFast") && thinkFastCount < 20)
                {
                    thinkFastCount++;
                    tempHighscores.Add(hScore);
                }
            }
            aggregateHighscores_ = tempHighscores;
        }

          /*
        if (aggregateHighscores_.Count > MaxAggregateCount)
          aggregateHighscores_.RemoveRange(MaxAggregateCount, aggregateHighscores_.Count - MaxAggregateCount);
          */
        Highscore prev = aggregateHighscores_[0];
        for (int i = 1, n = aggregateHighscores_.Count; i != n; ++i)
        {
          Highscore cur = aggregateHighscores_[i];
          if (prev.Score == cur.Score && prev.Gamer == cur.Gamer && prev.When == cur.When)
          {
            //  remove a duplicate
            if (cur.IsLocal)
            {
              //  if I got one from a remote guy, and another from locally, then 
              //  keep the local score
              aggregateHighscores_.RemoveAt(i-1);
            }
            else
            {
              aggregateHighscores_.RemoveAt(i);
            }
            --i;
            --n;
          }
          else
          {
            prev = cur;
          }
        }
      }
      hasReadNetwork_ = previousHosts_.Count > 0;
      if (HighscoresChanged != null && aggregateHighscores_.Count > 0)
        HighscoresChanged(this, aggregateHighscores_[0]);
    }

    //  You can be told when the set of known highscores changes.    
    public event HighscoresChanged HighscoresChanged;
  }

  public delegate void HighscoresChanged(HighscoreComponent sender, Highscore highestScore);

  public class Highscore : IComparable<Highscore>
  {
    public DateTime When;
    public string Gamer;
    public long Score;
    public string Message;
    public bool IsLocal;

    public string Encode()
    {
      return String.Format("{0};{1};{2};{3}", When.Ticks, Gamer, Score, Message);
    }
    
    public bool Decode(string str)
    {
      try
      {
        if (str == null) return false;
        string[] data = str.Split(';');
        if (data.Length != 4) return false;
        When = new DateTime(Int64.Parse(data[0]));
        Gamer = data[1];
        Score = Int64.Parse(data[2]);
        Message = data[3];
        return true;
      }
      catch (System.Exception x)
      {
        System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HighscoreComponent.HostType, x.Message));
        return false;
      }
    }

    public bool Write(PacketWriter wr)
    {
      try
      {
        if (Gamer.Length > 32)
          Gamer = Gamer.Substring(0, 32);
        if (Message.Length > 32)
          Message = Message.Substring(0, 32);
        wr.Write((long)When.Ticks);
        wr.Write(Gamer);
        wr.Write(Score);
        wr.Write(Message);
      }
      catch (System.Exception x)
      {
        System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HighscoreComponent.HostType, x.Message));
        return false;
      }
      return true;
    }

    public bool Read(PacketReader rd)
    {
      try
      {
        When = new DateTime(rd.ReadInt64());
        Gamer = rd.ReadString();
        Score = rd.ReadInt64();
        Message = rd.ReadString();
      }
      catch (System.Exception x)
      {
        System.Diagnostics.Trace.WriteLine(String.Format("{0}: {1}", HighscoreComponent.HostType, x.Message));
        return false;
      }
      return true;
    }

    #region IComparable<Highscore> Members

    public int CompareTo(Highscore other)
    {
      if (other == null) return -1;
      if (other.Score > Score) return 1;
      if (other.Score < Score) return -1;
      if (other.When > When) return 1;
      if (other.When < When) return -1;
      return other.Gamer.CompareTo(Gamer);
    }

    #endregion
  }
}
