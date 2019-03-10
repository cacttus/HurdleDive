using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Core
{
    //Run Fred, Run!
    //Fred was at Wal-Mart
    //He wanted to try on some underwear..
    //"I'm going to the women's section" said his wife..
    //Fred was in the dressing room.
    //A leprochaun appeared..
    //"Oh no, a shorts-stealing Wal-Mart Leprochaun" said Fred.
    //"I got your shorts, nana nana boo boo" snickered the Leprochaun.
    //The leprochaun started to run. "NO!" shouted Fred.
    //Fred chased after the leprochaun.
    //But he didn't have any shorts..
    //Everyone in Wal-Mart saw Fred.
    //And there happened to be an anti-nudist Mob here too.
    //"This guy is a deviant get him!" said the Mob
    //The mob chased Fred.
    //"Oh no!" said fred.
    //Fred started to run..

    //If you are in the air, tap, then change direction.
    public enum GameState
    {
        Play,
        GameOver
    }
    public class Tile : GameObject
    {
        public Tile(World w) : base(w) { }
    }
    public class Hurdle : GameObject
    {
        public int Height { get; set; } = -1;
        public Hurdle(World w) : base(w) { this.CollisionFlags = 0x0 | Core.CollisionFlags.CollidePixel; }
    }
    public class Coin : GameObject
    {
        public Coin(World w) : base(w)
        {
            Animate = true;
        }
    }
    //Business Rules
    //we generate "chunks" and also collide locally in them
    //x pos of the viewport determines which tilesa re visible.
    //So these have a point at the bottom where they sit on platforms.
    //wE ARE 7 tiles HIGH, but we Show as MANY CHUNKS THAT FIT ON THE SCREE.
    //If the player enters Tile 0, OR tile 6 MOVE TEH SCREEN UP
    public class Chunk
    {
        public const int iChunkWidthTiles = 5;
        public const int iChunkTilesYMin = -2;// - 2
        public const int iChunkTilesYMax = 9; // + 7(scren) + 2

        //Divide into chunks to avoid too much Collision bullshit
        public Box2f Bounds { get; set; }
        public List<Tile> Tiles { get; private set; } = new List<Tile>();
        World World;
        public List<Hurdle> Hurdles { get; private set; } = new List<Hurdle>();
        public List<Coin> Coins { get; private set; } = new List<Coin>();
        static int lastHeight = 0;
        public int ChunkId { get; private set; }

        bool StartZone = false;
        public Chunk(World w, int chunkId, bool startZone, Box2f bounds)
        {
            StartZone = startZone;
            ChunkId = chunkId;

            Bounds = bounds;

            int iChunkType = (int)Globals.Random(0, 10);

            World = w;

            if (startZone == false)
            {
                GenHurdles();
            }
            GenGround();
        }
        public void GenGround()
        {
            for (int i = 0; i < iChunkWidthTiles; ++i)
            {
                int iBaseGrassHeight = 5;
                Tile ob;

                ob = new Tile(World);
                ob.CollisionFlags = 0x00 | CollisionFlags.CollideTop;
                ob.Animate = false;
                ob.Frame = World.Res.Tiles.GetSpriteFrame(World.Res.SprGrassTiles, 1);
                ob.Pos = new vec2(World.Res.Tiles.TileWidthPixels * i, World.Res.Tiles.TileHeightPixels * iBaseGrassHeight);
                ob.Pos += new vec2(Bounds.Min.x, Bounds.Min.y);
                Tiles.Add(ob);

                for (int j = iBaseGrassHeight + 1; j < iChunkTilesYMax; j++)
                {
                    ob = new Tile(World);
                    ob.CollisionFlags = 0;
                    ob.Animate = false;
                    ob.Frame = World.Res.Tiles.GetSpriteFrame(World.Res.SprGrassTiles, 4);
                    ob.Pos = new vec2(World.Res.Tiles.TileWidthPixels * i, World.Res.Tiles.TileHeightPixels * j);
                    ob.Pos += new vec2(Bounds.Min.x, Bounds.Min.y);
                    Tiles.Add(ob);
                }

            }
        }

        public void GenHurdles()
        {
            for (int iPix = 0; iPix < Chunk.iChunkWidthTiles * World.Res.Tiles.TileWidthPixels; iPix++)
            {
                World.LastHurdle_X -= 1;
                if (World.LastHurdle_X <= 0)
                {
                    //Sub a bit

                    float minDist1 = World.Res.Tiles.TileWidthPixels * 2.75f;//3 is just about too big 2.5 too small
                    float minDist2 = World.Res.Tiles.TileWidthPixels * 2.0f;//3 is just about too big 2.5 too small

                    if (World.SubRound == false)
                    {
                        World.LastHurdle_Max -= 1;
                        if (World.LastHurdle_Max < minDist1)
                        {
                            World.LastHurdle_Max = minDist1;
                            World.SubRound = true;
                        }
                    }
                    else
                    {
                        //SubRound - So in order to make the game eventually unbeatable (drag on forever)
                        //keep subtracting teeny tiny values
                        World.LastHurdle_Max -= 0.005f;
                        if (World.LastHurdle_Max < minDist2)
                        {
                            World.LastHurdle_Max = minDist2;
                        }
                    }


                    World.LastHurdle_X = World.LastHurdle_Max;
                    CreateRandomHurdle(iPix);
                }

            }

        }
        public void CreateRandomHurdle(float xPos)
        {
            int height = Globals.RandomInt(0, 12 * 2);//0 - 12*2

            //prevent non-jumpable
            //Make sure we can jump over the big ones by making small ones before
            if (height > 8 && lastHeight > 8) { height = Globals.RandomInt(0, 1); }

            lastHeight = height;

            //remaining height
            int rh = -(height % 12);

            Hurdle h;
            if (height > 0 && height < 12)
            {
                h = new Hurdle(World);
                h.Height = height;
                h.Animate = false;
                h.Frame = World.Res.Tiles.GetSpriteFrame(World.Res.SprHurdle, 1);
                h.Pos = new vec2(xPos, World.Res.Tiles.TileHeightPixels * 4);
                h.Pos += new vec2(Bounds.Min.x, Bounds.Min.y);
                Hurdles.Add(h);

                h = new Hurdle(World);
                h.Animate = false;
                h.Frame = World.Res.Tiles.GetSpriteFrame(World.Res.SprHurdle, 0);
                h.Pos = new vec2(xPos, World.Res.Tiles.TileHeightPixels * 4 + rh);
                h.Pos += new vec2(Bounds.Min.x, Bounds.Min.y);
                Hurdles.Add(h);


            }
            else if (height >= 12)
            {
                h = new Hurdle(World);
                h.Height = height;
                h.Animate = false;
                h.Frame = World.Res.Tiles.GetSpriteFrame(World.Res.SprHurdle, 1);
                h.Pos = new vec2(xPos, World.Res.Tiles.TileHeightPixels * 4);
                h.Pos += new vec2(Bounds.Min.x, Bounds.Min.y);
                Hurdles.Add(h);

                h = new Hurdle(World);
                h.Animate = false;
                h.Frame = World.Res.Tiles.GetSpriteFrame(World.Res.SprHurdle, 1);
                h.Pos = new vec2(xPos, World.Res.Tiles.TileHeightPixels * 3);
                h.Pos += new vec2(Bounds.Min.x, Bounds.Min.y);
                Hurdles.Add(h);


                h = new Hurdle(World);
                h.Animate = false;
                h.Frame = World.Res.Tiles.GetSpriteFrame(World.Res.SprHurdle, 0);
                h.Pos = new vec2(xPos, World.Res.Tiles.TileHeightPixels * 3 + rh);
                h.Pos += new vec2(Bounds.Min.x, Bounds.Min.y);
                Hurdles.Add(h);

            }
            else if (height == 0)
            {
                h = new Hurdle(World);
                h.Height = height;
                h.Animate = false;
                h.Frame = World.Res.Tiles.GetSpriteFrame(World.Res.SprHurdle, 0);
                h.Pos = new vec2(xPos, World.Res.Tiles.TileHeightPixels * 4);
                h.Pos += new vec2(Bounds.Min.x, Bounds.Min.y);
                Hurdles.Add(h);
            }
        }
        public void Update(Input inp, float dt)
        {
            foreach (Coin c in Coins)
            {
                c.Update(inp, dt);
            }
        }
    }
    public class Guy : GameObject
    {
        public List<vec2> LastPos = new List<vec2>();
        public Guy(WorldBase w, string spr) : base(w, spr) { }
    }
    public class World : WorldBase
    {
        Guy Guy;
        public Res Res { get; private set; }
        public GameState GameState = GameState.Play;
        int iPlayCount = 0;

        ScreenShake ScreenShake = new ScreenShake();
        public bool SubRound { get; set; } = false;

        Dictionary<int, Chunk> chunks = new Dictionary<int, Chunk>();

        public float LastHurdle_X { get; set; }
        public float LastHurdle_Max { get; set; }
        float hangtime = 0.5f;//Max amount of time we can hang in air
        float jumpSpeed = 90;
        float curhangtime = 0;
        bool playerDown = false;
        bool canJump = true;
        ButtonBase JumpButton = new ButtonBase();
        float GuyRoll = 0.0f;
        bool overHurdle = false;
        float curDist = 0;
        SoundEffectInstance jumpSound = null;
        int iFrame = 0;
        float startPosDist = 0;
        float introTextTime;
        bool lastPlayerDown = false;
        float fLastScore = 0;
        float fCurHigh = 0;
        
        public World(Screen screen, Res r) : base(screen)
        {
            Res = r;
        }

        public void StartGame()
        {
            Screen.Game.AdMan.HideAd("MainAd");

            if (curDist != 0.0f)
            {
                fLastScore = curDist;
            }

            SubRound = false;
            //start/restart the game
            GameState = GameState.Play;
            introTextTime = 0;

            chunks.Clear();
            iPlayCount++;

            (Screen as GameScreen).RetryButton.Visible = false;
            (Screen as GameScreen).bShowNewRecord = false;
            (Screen as GameScreen).bShowNice = false;

            startPosDist = -1;//Haven't started yet

            Guy = new Guy(this, Res.SprGuy);
            Guy.HitBoxes.Add(new Rectangle());
            Guy.Pos.x = 1;
            Guy.Pos.y = 1;
            Guy.Origin = new vec2(6, 6);
            Guy.Animate = true;

            Gravity = new vec2(0, 12 * 50);
            Guy.Acc = new vec2(0, 0);
            Guy.Vel = new vec2(50.0f, 0);

            Screen.Viewport.Pos = new vec2(0, 0);

            //Initial Width
            LastHurdle_X = LastHurdle_Max = Res.Tiles.TileWidthPixels * Chunk.iChunkWidthTiles * 0.9f;

            //Try to prevent the beginning lag by making the world upfront
            Update(0.0f);
        }

        public override void Update(float dt)
        {
            ScreenShake.Update(dt);
            introTextTime += dt;

            MakeWorld(dt);

            DoJumps(dt);

            CheckGameOver(dt);

            HandlePhysics(dt);

            UpdateViewport(dt);
        }
        public void MakeWorld(float dt)
        {
            //Make World
            float fChunkWidthPixels = Chunk.iChunkWidthTiles * Res.Tiles.TileWidthPixels;
            int iChunkBase = (int)(Screen.Viewport.Pos.x / fChunkWidthPixels);
            int nChunks = (int)(Screen.Viewport.WidthPixels / fChunkWidthPixels);

            for (int iCh = -1; iCh < nChunks +2; ++iCh)// -1/+1 covers the whole screen
            {
                int chunkId = iChunkBase + iCh;
                Chunk ch = null;
                if (chunks.TryGetValue(chunkId, out ch) == false)
                {
                    bool startZone = chunkId == -1 || chunkId == 0 || chunkId ==1 || chunkId==2;

                    Chunk newChunk = new Chunk(this, chunkId, startZone, new Box2f(
                        chunkId * fChunkWidthPixels,
                        Chunk.iChunkTilesYMin * Res.Tiles.TileHeightPixels,
                        fChunkWidthPixels,
                        Chunk.iChunkTilesYMax * Res.Tiles.TileHeightPixels));

                    chunks.Add(chunkId, newChunk);
                }
            }

            List<Chunk> deadChunks = new List<Chunk>();
            foreach (Chunk c in chunks.Values)
            {
                c.Update(Screen.Game.Input, dt);

                if (c.Bounds.Max.x < Screen.Viewport.Pos.x)
                {
                    deadChunks.Add(c);
                }
            }
            //I think this is causing issues with hurdles too lcose
            //foreach(Chunk d in deadChunks)
            //{
            //    chunks.Remove(d.ChunkId);
            //}
            //deadChunks.Clear();
            //  GC.Collect();

        }

        DelayedAction ShowRetryMenuAction = null;

        private void ShowRetryMenu(float dt)
        {
            if (ShowRetryMenuAction != null)
            {
                ShowRetryMenuAction.Update(dt);
            }

            if (ShowRetryMenuAction == null)
            {
                if ((Screen as GameScreen).RetryButton.Visible == false)
                {
                    float hs = (Screen.Game as MainGame).GetHighScore();
                    
                    if (hs < curDist)
                    {

                        //Play actions
                        ShowRetryMenuAction = new DelayedAction(() =>
                        {
                            (Screen as GameScreen).RetryButton.Visible = true;
                            //Save game,, and if... greater
                            (Screen as GameScreen).bShowNewRecord = true;
                            Res.Audio.PlaySound(Res.SfxCoinGet);
                            ShowRetryMenuAction = null;
                        }, 1.0f, false);


                        (Screen.Game as MainGame).SetHighScore(curDist);

                    }
                    else if(curDist > 10)
                    {
                        //Play actions
                        ShowRetryMenuAction = new DelayedAction(() =>
                        {
                            (Screen as GameScreen).RetryButton.Visible = true;
                            (Screen as GameScreen).bShowNice = true;
                            Res.Audio.PlaySound(Res.SfxNice);
                            ShowRetryMenuAction = null;
                        }, 1.0f, false);
                    }
                    else
                    {
                        //Just show
                        (Screen as GameScreen).RetryButton.Visible = true;
                        Screen.Game.AdMan.ShowAd("MainAd");
                        
                    }

                    fCurHigh = (Screen.Game as MainGame).GetHighScore();
                }
                else
                {
                    if (JumpButton.TouchState == TouchState.Release)
                    {
                        StartGame();
                    }
                }

            }

        }
        public void DoJumps(float dt)
        {
            bool bDown = false;
            if (!Screen.Game.Input.UIClicked)
            {

                if (Screen.Game.GameSystem.GetPlatform() == Platform.Android ||
                    Screen.Game.GameSystem.GetPlatform() == Platform.iOS)
                {
                    bDown = Screen.Game.Input.Global.TouchState == TouchState.Press || Screen.Game.Input.Global.TouchState == TouchState.Down;
                }
                else
                {
                    bDown = Keyboard.GetState().IsKeyDown(Keys.Space);
                }
            }


            //Jump
            JumpButton.Update(bDown);
            if (GameState == GameState.Play)
            {
                if (startPosDist >= 0)
                {

                    curDist = ((Guy.Pos.x - startPosDist) / (Res.Tiles.TileWidthPixels * 2));
                }
                else
                {
                    curDist = 0;

                }
                if (curDist < 0) { curDist = 0; }

                if ((JumpButton.TouchState == TouchState.Press || JumpButton.TouchState == TouchState.Down))
                {
                    if (canJump == true)
                    {

                        if (curhangtime < hangtime)
                        {
                            curhangtime += dt;
                            Guy.Vel = new vec2(Guy.Vel.x, -jumpSpeed);

                            if (JumpButton.TouchState == TouchState.Press)
                            {
                                jumpSound = Res.Audio.PlaySound(Res.SfxJump);
                            }
                        }
                        else
                        {

                            canJump = false;
                            if (jumpSound != null)
                            {
                                jumpSound.Stop();
                            }
                        }
                    }
                    else if (JumpButton.TouchState == TouchState.Press)
                    {
                        //Increase Gravity Temporarily
                        Res.Audio.PlaySound(Res.SfxWhiroop);
                        Guy.Vel = new vec2(Guy.Vel.x, Guy.Vel.y + 180);
                        Guy.Animate = false;
                        Guy.Frame = Res.Tiles.GetSpriteFrame(Res.SprGuyDown, 0);
                    }

                }
                else if (JumpButton.TouchState == TouchState.Release || JumpButton.TouchState == TouchState.Up)
                {
                    if (jumpSound != null)
                    {
                        jumpSound.Stop();
                    }
                    canJump = false;
                    curhangtime = hangtime;
                    if (playerDown)
                    {
                        canJump = true;
                    }
                }
            }
        }

        public void CheckGameOver(float dt)
        {
            if (GameState == GameState.GameOver)
            {

                if (GuyRoll > 0.0f)
                {
                    GuyRoll = GuyRoll - 4.0f * dt;
                    if (GuyRoll < 0.0f) GuyRoll = 0.0f;

                }
                if (Guy.Vel.x > 0.0f)
                {
                    Guy.Vel.x -= 17.0f * dt;
                    if (Guy.Vel.x < 0) { Guy.Vel.x = 0; }
                }
                Guy.RotationDelta = GuyRoll;

                if (GuyRoll <= 0.1f)
                {
                    ShowRetryMenu(dt);
                }

            }


        }
        public void HandlePhysics(float dt)
        {
            //Guy Physics
            World w = this;
            Guy.Update(this.Screen.Game.Input, dt, () => { });

            //The hit pixel
            Guy.Vel += Guy.Acc * dt;
            Guy.Vel += w.Gravity * dt;
            vec2 next = Guy.Pos + Guy.Vel * dt;

            vec2 hitBot = new vec2(Guy.Pos.x + 6, Guy.Pos.y);
            vec2 nextBot = hitBot + Guy.Vel * dt;
            vec2 hitTop = new vec2(Guy.Pos.x + 6, Guy.Pos.y - Res.Tiles.TileHeightPixels + 1);
            vec2 nextTop = hitTop + Guy.Vel * dt;
            lastPlayerDown = playerDown;
            playerDown = false;

            //Microsoft.Xna.Framework.Ray ray;
            //ray.Position = new Vector3(hitBot.x, hitBot.y, 0);
            //ray.Direction = new Vector3(nextBot.x, nextBot.y, 0) - ray.Position;
            //ray.Direction.Normalize();

            //A loose fitting box around the guy.
            Box2f guyBox = new Box2f(Guy.Pos + new vec2(3, 3), Guy.Pos + new vec2(3, 3) + new vec2(6, 6));
            Box2f guySpeedBox = guyBox;
            guySpeedBox.ExpandByPoint(next);

            List<Chunk> collided = new List<Chunk>();
            foreach (Chunk chunk in chunks.Values)
            {
                if (chunk.Bounds.BoxIntersect_EasyOut_Inclusive(guySpeedBox))
                {
                    collided.Add(chunk);
                }
            }


            GameObject closest = null;
            bool lastOverHurdle = overHurdle;
            overHurdle = false;
            bool played = false;
            foreach (Chunk chunk in collided)
            {
                bool hitHurdle = false;

                List<Hurdle> over = new List<Hurdle>();

                foreach (Hurdle hurd in chunk.Hurdles)
                {
                    vec2 vpos1 = new vec2(hurd.Pos.x + 5, hurd.Pos.y + 3); //top of hurdle.
                    vec2 vpos2 = new vec2(hurd.Pos.x + 5, hurd.Pos.y + 9); //top of hurdle.

                    if ((hitBot.x >= hurd.Pos.x) && (hitBot.x < hurd.Pos.x + 12) /*&& overHurdle == false*/)
                    {
                        if (startPosDist < 0)
                        {
                            startPosDist = hitBot.x;
                        }

                        overHurdle = true;
                        over.Add(hurd);
                    }

                    if (guyBox.ContainsPointInclusive(vpos1))
                    {
                        hitHurdle = true;
                    }
                    else if (guyBox.ContainsPointInclusive(vpos2))
                    {
                        hitHurdle = true;
                    }
                }

                if (Guy.Pos.y > 500 || Guy.Pos.y < -500)
                {
                    //out of bounds
                    hitHurdle = true;
                }

                //Game Over!
                if (hitHurdle && GameState != GameState.GameOver)
                {
                    GameState = GameState.GameOver;
                    //  Res.Audio.PlaySound(Res.SfxFail);
                    Res.Audio.PlaySound(Res.SfxCrash);
                    GuyRoll = 10.5f;
                    Guy.Animate = false;
                    ScreenShake.Shake(2);

                }

                if (GameState != GameState.GameOver)
                {
                    if (lastOverHurdle == false && overHurdle == true && played == false)
                    {
                        //Change hurdle to greeen
                        foreach (Hurdle h in over)
                        {
                            if (h.Frame.Index == 0) { h.Frame = Res.Tiles.GetSpriteFrame(Res.SprHurdle, 3); }
                            if (h.Frame.Index == 1) { h.Frame = Res.Tiles.GetSpriteFrame(Res.SprHurdle, 4); }
                            if (h.Frame.Index == 2) { h.Frame = Res.Tiles.GetSpriteFrame(Res.SprHurdle, 5); }
                        }
                        Res.Audio.PlaySound(Res.SfxGet);
                        played = true;
                    }
                }

                foreach (Tile tile in chunk.Tiles)
                {
                    //Bottom pixel
                    if ((tile.CollisionFlags & CollisionFlags.CollideTop) > 0)
                    {
                        if (hitBot.x >= tile.Pos.x && hitBot.x < tile.Pos.x + 12)
                        {
                            if (Guy.Vel.y >= 0)
                            {
                                //Collide with top of tile.
                                int tpy = (int)tile.Pos.y - Res.Tiles.TileHeightPixels + 1;
                                if (hitBot.y < tpy && nextBot.y >= tpy)
                                {
                                    //check closest tile
                                    if ((closest == null) || (Math.Abs(tile.Pos.y - hitBot.y) < Math.Abs(closest.Pos.y - hitBot.y)))
                                    {
                                        //Hit
                                        closest = tile;
                                        next.y = tpy - 1;
                                        playerDown = true;
                                        if (lastPlayerDown == false)
                                        {
                                            Res.Audio.PlaySound(Res.SfxLand);
                                        }
                                        curhangtime = 0;

                                        if (GameState != GameState.GameOver)
                                        {
                                            Guy.Animate = true;//Set back to aniamte if we are diving
                                        }
                                        //Guy.Frame = Res.Tiles.GetSpriteFrame(Res.SprGuyDown, 0);

                                    }
                                }
                            }
                        }
                    }

                    ////Top pixel
                    //if (hitTop.X >= tile.Pos.X && hitTop.X < tile.Pos.X + 12)
                    //{
                    //    if (Guy.Vel.Y < 0)
                    //    {
                    //        //Collide with top of tile.
                    //        int tpy = (int)tile.Pos.Y;
                    //        if (hitTop.Y >= tpy && nextTop.Y < tpy)
                    //        {
                    //            //Hit
                    //             next.Y = tpy + (hitBot.Y-hitTop.Y);
                    //            //playerDown = true;
                    //            curhangtime = hangtime;
                    //        }
                    //    }
                    //}
                }

            }
            if (Guy != null && (Guy as Guy).LastPos != null)
            {
                (Guy as Guy).LastPos.Add(Guy.Pos);
                if ((Guy as Guy).LastPos.Count > 5)
                {
                    (Guy as Guy).LastPos.RemoveAt(0);
                }
            }
            Guy.Pos = next;


        }
        public void UpdateViewport(float dt)
        {
            //Viewport Update
            //Makes ure the viewport doesn't go past these values.
            Screen.Viewport.Pos = new vec2(
                Guy.Pos.x - Res.Tiles.TileWidthPixels * Screen.Viewport.TilesWidth * 0.5f + Res.Tiles.TileWidthPixels * 0.5f
                , -Res.Tiles.TileHeightPixels * 2.0f) + ScreenShake.ScreenShakeOffset; ;

            //Guy Bounds.
            float YBorder = Res.Tiles.TileHeightPixels;
            if (Guy.Pos.y < Screen.Viewport.Pos.y + YBorder)
            {
                Screen.Viewport.Pos = new vec2(Screen.Viewport.Pos.x, Guy.Pos.y - YBorder);
            }
            if (Guy.Pos.y > Screen.Viewport.Pos.y + Screen.Viewport.HeightPixels - YBorder)
            {
                Screen.Viewport.Pos = new vec2(Screen.Viewport.Pos.x, Guy.Pos.y - Screen.Viewport.HeightPixels + YBorder);
            }

            //We need second bounds check for the actual "game area'
            //If the guy falls out of the screen where we don't want to go
        }
        public override void Draw(SpriteBatch sb)
        {
            iFrame++;
            Frame fback = Res.Tiles.GetSpriteFrame(Res.SprBackdrop, 0);
            //Backdrop
            for (int iBack = 0; iBack < Screen.Viewport.TilesWidth + 1; ++iBack)
            {
                sb.Draw(Res.Tiles.Texture,
                Screen.Viewport.WorldToDevice(new vec2(Screen.Viewport.Pos.x + 12 * iBack, Screen.Viewport.Pos.y), new vec2(13, 12 * 7)),
                fback.R, Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.0f);
            }

            //Parallax
            float shift0 = Screen.Viewport.Pos.x % (12 * 2);
            float shift1 = Screen.Viewport.Pos.x % (12 * 2);
            float shift2 = Screen.Viewport.Pos.x % (12 * 2);
            float yBase = -24;//This is the base pos of the guy ont he ground of the VIEWPORT

            float delta = (yBase - Screen.Viewport.Pos.y) / yBase;
            float d0 = delta * -5.0f;
            float d1 = delta * -10.00f;
            float d2 = delta * -15.00f;

            vec2 wh = new vec2(12 * 2  , 12 * 3);
            float yPos = Screen.Viewport.Pos.y + (Screen.Viewport.TilesHeight - 3) * 12;
            Frame par;
            for (int iBack = -1; iBack < Screen.Viewport.TilesWidth / 2 + 1; ++iBack)
            {
                float xPos = Screen.Viewport.Pos.x + (12 * iBack * 2);
                par = Res.Tiles.GetSpriteFrame(Res.SprBackPar, 0);
                sb.Draw(Res.Tiles.Texture,
                    Screen.Viewport.WorldToDevice(new vec2(xPos - shift0, yPos + d0), wh),
                    par.R
                    , Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.0f);
            }
            for (int iBack = -1; iBack < Screen.Viewport.TilesWidth / 2 + 1; ++iBack)
            {
                float xPos = Screen.Viewport.Pos.x + (12 * iBack * 2);
                par = Res.Tiles.GetSpriteFrame(Res.SprBackPar, 1);
                sb.Draw(Res.Tiles.Texture,
                    Screen.Viewport.WorldToDevice(new vec2(xPos - shift1, yPos + d1), wh),
                    par.R
                    , Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.0f);
            }
            for (int iBack = -1; iBack < Screen.Viewport.TilesWidth / 2 + 1; ++iBack)
            {
                float xPos = Screen.Viewport.Pos.x + (12 * iBack * 2);
                par = Res.Tiles.GetSpriteFrame(Res.SprBackPar, 2);
                sb.Draw(Res.Tiles.Texture,
                    Screen.Viewport.WorldToDevice(new vec2(xPos - shift2, yPos + d2), wh),
                    par.R
                    , Color.White, 0.0f, new Vector2(0, 0), SpriteEffects.None, 0.0f);
            }


            //Sun
            Frame sun = Res.Tiles.GetSpriteFrame(Res.SprSun, 0);
            vec2 sunpos = new vec2(Res.Tiles.TileWidthPixels * ((float)Screen.Viewport.TilesWidth - 1.4f), Res.Tiles.TileHeightPixels * -2f);
            sunpos.x += Screen.Viewport.Pos.x;
            Screen.DrawFrame(sb, sun, sunpos, new vec2(sun.R.Width, sun.R.Height), Color.White * 0.4f);

            //Grass, Frames
            foreach (Chunk ch in chunks.Values)
            {
                foreach (GameObject tile in ch.Tiles)
                {
                    Screen.DrawFrame(sb, tile.Frame, tile.Pos, new vec2(tile.Frame.R.Width, tile.Frame.R.Height), Color.White);
                }
                foreach (Hurdle h in ch.Hurdles)
                {
                    Screen.DrawFrame(sb, h.Frame, h.Pos, new vec2(h.Frame.R.Width, h.Frame.R.Height), Color.White);

                    //Height images.
                    //if (h.Height >= 0)
                    //{
                    //    Frame nbk = Res.Tiles.GetSpriteFrame(Res.SprNumberBk, 0);
                    //    Screen.DrawFrame(sb, nbk, h.Pos + new vec2(0,12), new vec2(nbk.R.Width, nbk.R.Height), Color.White);
                    //    string strHeight = h.Height.ToString();
                    //    float width2 = Screen.Viewport.WidthPixels * 0.05f;
                    //    Rectangle r = Screen.Viewport.WorldToScreen(h.Pos + new vec2(2,12), new vec2(12, 12));
                    //    sb.DrawString(
                    //      Res.Font,
                    //      strHeight,
                    //      (new vec2(r.X, r.Y)).toXNA(),
                    //      Color.Black,0.0f, new Vector2(0,0),
                    //      0.5f,
                    //      SpriteEffects.None,0.0f);
                    //}


                }


                foreach (Coin c in ch.Coins)
                {
                    if (c.Frame != null)
                    {
                        Screen.DrawFrame(sb, c.Frame, c.Pos, new vec2(c.Frame.R.Width, c.Frame.R.Height), Color.White);
                    }

                }
            }

            //Dude + Vapor trail
            //Vapor gets in the way
            //Show only when diving
            if (GameState != GameState.GameOver)
            {
                if (Guy.Animate == false)
                {
                    for(int i=0; i<Guy.LastPos.Count; ++i)
                    {
                        if (i!=0 && i %3 == 0)
                        {
                            float a = ((float)(i + 1) / Guy.LastPos.Count) * 0.4f;
                            Screen.DrawFrame(sb, Guy.Frame, Guy.LastPos[i], new vec2(Guy.Frame.R.Width, Guy.Frame.R.Height), Color.White * a);
                        }

                    }
                }
            }

            Screen.DrawFrame(sb, Guy.Frame, Guy.Pos + Guy.Origin, new vec2(Guy.Frame.R.Width, Guy.Frame.R.Height), Color.White, 1.0f, 1.0f, Guy.Rotation, Guy.Origin);

            if (GameState != GameState.GameOver)
            {
                //Distance
                string strDistance = curDist.ToString("0.0") + "m";
                float width = Res.Font.MeasureString(strDistance).X;
                Screen.DrawStringFit(sb, strDistance, Res.Font,
                    new vec2(Screen.Game.GraphicsDevice.Viewport.Width * 0.5f - width*0.5f, 0),//random math here
                    width);

                //Tap to play

                if (introTextTime < 3.0f && iPlayCount < 2)
                {
                    //if (iFrame % 5 == 0 || iFrame % 4 == 0)
                    {

                        string str;
                        if(Screen.Game.GameSystem.GetPlatform() == Platform.Desktop)
                        {

                            if (introTextTime < 1.5f)
                            {
                                str = "Press Space To Jump.";
                            }
                            else
                            {
                                str = "Press Again To Dive.";
                            }
                        }
                        else
                        {

                            if (introTextTime < 1.5f)
                            {
                                str = "Tap To Jump.";
                            }
                            else
                            {
                                str = "Tap Again To Dive.";
                            }
                        }

                        vec2 siz = Screen.Viewport.MeasureString(Res.Font, str);

                        sb.DrawString(
                          Res.Font2,
                          str,
                          Screen.Viewport.ScreenPixelsToScreenRaster(new vec2(Screen.Viewport.WidthPixels / 2 - siz.x * .5f, Screen.Viewport.HeightPixels / 2 - 10)).toXNA(),
                          Color.Black, 0.0f, new Vector2(0, 0),
                            1.0f,
                          SpriteEffects.None, 0.0f);
                    }
                }

            }
            else
            {
                //Distance
                string strDistance = curDist.ToString("0.0") + "m";
                vec2 siz = Screen.Viewport.MeasureString(Res.Font, strDistance);

                float scl = 2.0f;

                float outl_scl = 0.01f;
                vec2 outl = new vec2(Screen.Viewport.WidthPixels * outl_scl, Screen.Viewport.HeightPixels * outl_scl);
                vec2 pos = new vec2(Screen.Viewport.WidthPixels / 2 - siz.x, Screen.Viewport.HeightPixels / 2 - 10);

                DrawOutlineText(sb, strDistance, scl, pos, outl);

                //Draw teh last score
                if (fLastScore != 0.0f)
                {
                    strDistance = "Last:" + fLastScore.ToString("0.0") + "m";
                    siz = Screen.Viewport.MeasureString(Res.Font, strDistance);

                    scl = 0.5f;

                    outl_scl = 0.003f;
                    outl = new vec2(Screen.Viewport.WidthPixels * outl_scl , Screen.Viewport.HeightPixels * outl_scl );
                    pos = new vec2(Screen.Viewport.WidthPixels / 2 - siz.x * scl + Screen.Viewport.WidthPixels / 4
                        , Screen.Viewport.HeightPixels / 2 + 10);

                    DrawOutlineText(sb, strDistance, scl, pos, outl);
                }


                //Draw teh last score
                if (fCurHigh != 0.0f)
                {
                    strDistance = "Best:" + fCurHigh.ToString("0.0") + "m";
                    siz = Screen.Viewport.MeasureString(Res.Font, strDistance);

                    scl = 0.5f;

                    outl_scl = 0.003f;
                    outl = new vec2(Screen.Viewport.WidthPixels * outl_scl, Screen.Viewport.HeightPixels * outl_scl);
                    pos = new vec2(Screen.Viewport.WidthPixels / 2 - siz.x * scl - Screen.Viewport.WidthPixels / 4
                        , Screen.Viewport.HeightPixels / 2 + 10);

                    DrawOutlineText(sb, strDistance, scl, pos, outl);
                }

            }

            //Title at bottom left corner
            Frame title = Res.Tiles.GetSpriteFrame(Res.SprTitle, 0);
            Screen.DrawFrame(sb,
                title, Screen.Viewport.Pos + new vec2(0, Screen.Viewport.HeightPixels - (Screen.Viewport.HeightPixels) * 0.12f),
                new vec2(Screen.Viewport.WidthPixels * 0.15f, Screen.Viewport.HeightPixels * 0.12f),
                Color.White * 0.5f, 1.0f, 1.0f, 0, new vec2(0, 0));


            //         Screen.Draw((Screen.Game as MainGame).Res.Tiles.Texture, new Rectangle(10, 10, 100, 100), Color.White);
        }
        public void DrawOutlineText(SpriteBatch sb, String strDistance, float scl, vec2 pos, vec2 outl)
        {

            sb.DrawString(
          Res.Font,
          strDistance,
          Screen.Viewport.ScreenPixelsToScreenRaster(
              pos - new vec2(outl.x, 0)).toXNA(),
          Color.White, 0.0f, new Vector2(0, 0),
          scl,
          SpriteEffects.None, 0.0f);
            sb.DrawString(
            Res.Font,
            strDistance,
            Screen.Viewport.ScreenPixelsToScreenRaster(
              pos - new vec2(0, outl.y)).toXNA(),
            Color.White, 0.0f, new Vector2(0, 0),
            scl,
            SpriteEffects.None, 0.0f);
            sb.DrawString(
                Res.Font,
                strDistance,
                Screen.Viewport.ScreenPixelsToScreenRaster(
                  pos + new vec2(outl.x, 0)).toXNA(),
                Color.White, 0.0f, new Vector2(0, 0),
                scl,
                SpriteEffects.None, 0.0f);
            sb.DrawString(
            Res.Font,
            strDistance,
            Screen.Viewport.ScreenPixelsToScreenRaster(
              pos + new vec2(0, outl.y)).toXNA(),
            Color.White, 0.0f, new Vector2(0, 0),
            scl,
            SpriteEffects.None, 0.0f);


            sb.DrawString(
              Res.Font,
              strDistance,
              Screen.Viewport.ScreenPixelsToScreenRaster(pos).toXNA(),
              Color.Black, 0.0f, new Vector2(0, 0),
              scl,
              SpriteEffects.None, 0.0f);
        }

    }
    public class GameScreen : Screen
    {
        //High Score REset
        int nSoundPress = 0;
        int nSoundPressTime = 10;
        float resetTime = 0.0f;
        float resetTimeMax = 5.0f;


        World World;
        public MenuButton RetryButton { get; private set; }
        public MenuButton SoundButton { get; private set; }
        public MenuButton SoundButton2 { get; private set; }
        public bool bShowNewRecord { get; set; } = false;
        public bool bShowNice { get; set; } = false;

        public override void Init(GameBase game)
        {
            base.Init(game);
            World = new World(this, (game as MainGame).Res);

            MenuButtons.Add(RetryButton = new MenuButton(this,
                new vec2(4, 2), new vec2(4, 2),
                new vec2(4, 4), new vec2(4, 2),
                new vec2(
                    Viewport.WidthPixels / 2 - (Game.Res.Tiles.TileWidthPixels * 4 * 0.5f), Viewport.HeightPixels / 2 + 12),
                false));
            RetryButton.Click = (x) =>
            {
                //Don't put any extra logic in here. put in stargame
                World.Res.Audio.PlaySound(World.Res.SfxBlip);
                World.StartGame();

            };

            vec2 soundxy = new vec2(
                2,2//(Game.Res.Tiles.TileWidthPixels * 2) + 2, 
                //Viewport.HeightPixels - (Game.Res.Tiles.TileHeightPixels * 1) + 2
                );
            vec2 soundwh = new vec2(Game.Res.Tiles.TileWidthPixels - 4 ,
                Game.Res.Tiles.TileWidthPixels - 4);

            MenuButtons.Add(SoundButton = new MenuButton(this,
                new vec2(2, 15), new vec2(3, 3),
                new vec2(5, 15), new vec2(3, 3),
                soundxy,
                true,
                soundwh.x, soundwh.y,
                Color.White * 0.3f
                ));
            SoundButton.Click = (x) =>
            {
                //Don't put any extra logic in here. put in stargame
                World.Res.Audio.Enabled = false;
                SoundButton.Visible = false;
                SoundButton2.Visible = true;
                nSoundPress++;
 
            };
            MenuButtons.Add(SoundButton2 = new MenuButton(this,
                new vec2(8, 15), new vec2(3, 3),
                new vec2(11, 15), new vec2(3, 3),
                soundxy,
                false,
                soundwh.x, soundwh.y,
                Color.White * 0.3f
                ));
            SoundButton2.Click = (x) =>
            {
                //Don't put any extra logic in here. put in stargame
                World.Res.Audio.Enabled = true;
                SoundButton.Visible = true;
                SoundButton2.Visible = false;
                nSoundPress++;
                
            };

            World.StartGame();

        }
        public override void Update(float dt)
        {
            base.Update(dt);
            World.Update(dt);

            //High Score Reset
            if (nSoundPress > 0)
            {
                resetTime += dt;
                if(resetTime > resetTimeMax)
                {
                    nSoundPress = 0;
                    resetTime = 0;
                }
                if (nSoundPress > nSoundPressTime)
                {
                    //Reset the score
                    Game.Res.Audio.PlaySound(Game.Res.SfxCoinGet);
                    (Game as MainGame).SetHighScore(0.0f);
                    nSoundPress = 0;
                }
            }
            
        }
        public override void Draw()
        {
            base.BeginDraw();
            World.Draw(SpriteBatch);
            base.DrawMenu();//Must come after world.draw

            if (bShowNewRecord)
            {
                Frame nr = Game.Res.Tiles.GetSpriteFrame(Game.Res.SprNewRecord, 0);
                Rectangle screen = Viewport.WorldToDevice(Viewport.Pos + new vec2(-10, 12) + nr.PixelCenter(), new vec2(60, 12));
                SpriteBatch.Draw(Game.Res.Tiles.Texture,
                    screen,
                    nr.R, Color.White, (float)Math.PI * 0.10f, nr.PixelCenter().toXNA(), SpriteEffects.None, 0.0f);
            }
            else if (bShowNice)
            {
                Frame nr = Game.Res.Tiles.GetSpriteFrame(Game.Res.SprNice, 0);
                Rectangle screen = Viewport.WorldToDevice(
                    Viewport.Pos + new vec2(Viewport.WidthPixels * 0.5f,Viewport.HeightPixels*0.5f) - nr.PixelCenter(), 
                    new vec2(60, 20));
                SpriteBatch.Draw(Game.Res.Tiles.Texture,
                    screen,
                    nr.R, Color.White, /*(float)Math.PI * 0.10f*/0, nr.PixelCenter().toXNA(), SpriteEffects.None, 0.0f);
            }

            base.EndDraw();
        }
    }
    public class MainGame : GameBase
    {
        GraphicsDeviceManager graphics;
        GameScreen GameScreen;
        Screen _objCurScreen = null;
        GameData GameData;

        public float GetHighScore()
        {
            GameData.Load();
            return GameData.HighScore;
        }
        public void SetHighScore(float f)
        {
            GameData.HighScore = f;
            GameData.Save();
        }
        public void Init(AdMan adMan, bool bFullscreen, GameSystem gs)
        {
            GameSystem = gs;

            AdMan = adMan;

            GameData = new GameData(this);
            GameData.Load();

            graphics.IsFullScreen = bFullscreen;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
            graphics.ApplyChanges();

            Window.Title = "Hurdle Dive";


        }
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            //**on Android, fullscreen just hides the menu bar.
            //On Desktop - it's, well fullscreen..

            this.IsMouseVisible = true;//Dnr
                                       //We can't have constructors because of XAML
                                       //Fuxking XAML

            ///Variable time setp.
            /////So with fixed stepping, XNA will call Upate() multiple times to keep up.
            //Setting this to false, makes it variable, and XNA executes Update/Draw in succession
            this.IsFixedTimeStep = false;

        }
        protected override void Initialize()
        {
            base.Initialize();
        }
        protected override void LoadContent()
        {
            Res = new Res(Content);
            Res.Load(this.GraphicsDevice);

            //Do not do any usage of GameSystem ehre

            ShowScreen = ShowScreen.Game;
        }
        protected override void UnloadContent()
        {
        }
        float waitmax = 2.0f;
        float wait = 0.0f;
        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            base.Update(dt);

            //Change Screen.
            //if (ShowScreen == ShowScreen.Title)
            //{
            //    TitleScreen = new TitleScreen();
            //    TitleScreen.Init(this, _objTiles, _objAudio, _objFont, GraphicsDevice, _objGameData);
            //    _objCurScreen = TitleScreen;
            //}
            //else 
            //Wait state - to allow the screen to rotate
            if (wait >= waitmax)
            {
                if (ShowScreen == ShowScreen.Game)
                {

                    GameScreen = new GameScreen();
                    GameScreen.Init(this);
                    _objCurScreen = GameScreen;
                }

                ShowScreen = ShowScreen.None;
            }
            else
            {
                GameSystem.HideNav();
                wait += dt;
            }
            //else if (ShowScreen == ShowScreen.Tutorial)
            //{
            //    GameScreen = new GameScreen();
            //    GameScreen.Init(this, _objTiles, _objAudio, _objFont, GraphicsDevice, _objGameData);
            //    GameScreen.ShowTutorial();
            //    _objCurScreen = GameScreen;
            //}


            if (_objCurScreen != null)
            {
                _objCurScreen.Update(dt);
            }
            //If we touch screen, then hide the nav if it isn't hidden.
            if (Input.Global.TouchState == TouchState.Press || Input.Global.TouchState == TouchState.Release)
            {
                GameSystem.HideNav();

            }


            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                Exit();
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            if (_objCurScreen != null)
            {
                _objCurScreen.Draw();
            }

            base.Draw(gameTime);
        }
    }
}
