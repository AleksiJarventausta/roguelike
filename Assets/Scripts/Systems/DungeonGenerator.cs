//https://github.com/NagaChiang/unity-ecs-bsp-dungeon-generation
//used as a base for this generator.
using System;
using System.Collections;
using System.Collections.Generic;
using RogueLike.Dungeons;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random=UnityEngine.Random;
using Rect = RogueLike.Dungeons.Rect;
using Room = RogueLike.Dungeons.Room;
using RectNode = RogueLike.Dungeons.RectNode;

namespace RogueLike.Systems
{
    public class DungeonGenerator : MonoBehaviour
    {
        public Tilemap floor;
        public RuleTile floorTile;
        public int MaxRoomLength;
        public int MinRoomLength;
        public int ExtraPathNum;
        public float MinSplitRatio;
        public float MaxSplitRatio;
        public int DungeonWidth;
        public int DungeonHeight;
        public int seed;
        public int HallWayWidth;

        private List<Room> generatedRooms = new List<Room>();

        public List<Room> GetRooms()
        {
            return generatedRooms;
        }



        public void GenerateDungeon()

        {
            floor.ClearAllTiles();
            Random.InitState(seed);
            // Set all cells as wall
            SetWallAll(true);
            
            // Rooms
            Rect fullRect = new Rect(int2.zero, DungeonWidth, DungeonHeight);
            RectNode root = RectNode.CreateBspTree(fullRect, MaxRoomLength, MinSplitRatio, MaxSplitRatio);
            List<RectNode> leafs = root.GetLeafs();
            Dictionary<Rect, Room> rooms = new Dictionary<Rect, Room>();
            generatedRooms.Clear();
            foreach (RectNode leaf in leafs)
            {
                Room room = GenerateRoom(leaf.Rect, MinRoomLength);
                rooms.Add(leaf.Rect, room);
                generatedRooms.Add(room);
            }

            // Paths
            Stack<RectNode> nodeStack = new Stack<RectNode>();
            nodeStack.Push(root);
            while (nodeStack.Count > 0)
            {
                RectNode node = nodeStack.Pop();
                if (node.LeftNode == null && node.RightNode == null)
                {
                    continue;
                }

                Room roomLeft = rooms[node.LeftNode.GetRandomLeaf().Rect];
                Room roomRight = rooms[node.RightNode.GetRandomLeaf().Rect];
                GeneratePath(roomLeft, roomRight);

                nodeStack.Push(node.LeftNode);
                nodeStack.Push(node.RightNode);
            }

            // Extra paths
            if (root != null && root.LeftNode != null && root.RightNode != null)
            {
                for (int count = 0; count < ExtraPathNum; count++)
                {
                    Room roomLeft = rooms[root.LeftNode.GetRandomLeaf().Rect];
                    Room roomRight = rooms[root.RightNode.GetRandomLeaf().Rect];
                    GeneratePath(roomLeft, roomRight);
                }
            }
        }

        private Room GenerateRoom(Rect area, int minLength)
        {
            int width = Random.Range(Mathf.Min(minLength, area.Width), area.Width + 1);
            int height = Random.Range(Mathf.Min(minLength, area.Height), area.Height + 1);
            int2 lowerLeftPos = area.LowerLeftPos + new int2(Random.Range(0, area.Width - width), Random.Range(0, area.Height - height));
            Rect roomRect = new Rect(lowerLeftPos, width, height);
            DigInnerArea(roomRect);

            Room room = new Room(roomRect);
            return room;
        }
        void DigHallway(Rect rect)
        {
            DigArea(rect, floorTile);
        }

        private void GeneratePath(Room room1, Room room2)
        {
            Rect roomRect1 = room1.GetRect();
            Rect roomRect2 = room2.GetRect();
            int2 pos1 = roomRect1.GetRandomInnerPosition();
            int2 pos2 = roomRect2.GetRandomInnerPosition();
            int2 offset = pos2 - pos1;
            int horizontalLength = Mathf.Abs(offset.x) + 1;
            int verticalLength = Mathf.Abs(offset.y) + 1;
            bool isHorizontalFirst = Random.value > 0.5f;
            if (isHorizontalFirst)
            {
                // Horizontal first
                if (offset.x >= 0)
                {
                    // Right
                    DigHallway(new Rect(pos1, horizontalLength, HallWayWidth));

                    if (offset.y >= 0)
                    {
                        // Up
                        DigHallway(new Rect(pos2.x, pos1.y, HallWayWidth, verticalLength));
                    }
                    else
                    {
                        // Down
                        DigHallway(new Rect(pos2.x, pos2.y, HallWayWidth, verticalLength));
                    }
                }
                else
                {
                    // Left
                    DigHallway(new Rect(pos2.x, pos1.y, horizontalLength, HallWayWidth));

                    if (offset.y >= 0)
                    {
                        // Up
                        DigHallway(new Rect(pos2.x, pos1.y, HallWayWidth, verticalLength));
                    }
                    else
                    {
                        // Down
                        DigHallway(new Rect(pos2.x, pos2.y, HallWayWidth, verticalLength));
                    }
                }
            }
            else
            {
                // Vertical first
                if (offset.y >= 0)
                {
                    // Up
                    DigHallway(new Rect(pos1, HallWayWidth, verticalLength));

                    if (offset.x >= 0)
                    {
                        // Right
                        DigHallway(new Rect(pos1.x, pos2.y, horizontalLength, HallWayWidth));
                    }
                    else
                    {
                        // Left
                        DigHallway(new Rect(pos2, horizontalLength, HallWayWidth));
                    }
                }
                else
                {
                    // Down
                    DigHallway(new Rect(pos1.x, pos2.y, HallWayWidth, verticalLength));

                    if (offset.x >= 0)
                    {
                        // Right
                        DigHallway(new Rect(pos1.x, pos2.y, horizontalLength, HallWayWidth));
                    }
                    else
                    {
                        // Left
                        DigHallway(new Rect(pos2, horizontalLength, HallWayWidth));
                    }
                }
            }
        }

        private void DigArea(Rect rect, RuleTile tile)
        {
            List<int2> positions = rect.GetPositions();
            foreach (int2 pos in positions)
            {
                floor.SetTile(new Vector3Int(pos.x, pos.y, 0), tile);
            }
        }

        private void DigInnerArea(Rect rect)
        {
            foreach (int2 pos in rect.GetPositions())
            {
                floor.SetTile(new Vector3Int(pos.x, pos.y, 0), floorTile);
            }
        }

        private void SetWall(int2 pos, bool isWall)
        {
            floor.SetTile(new Vector3Int(pos.x, pos.y, 0), floorTile);
        }

        private void SetWallAll(bool isWall)
        {
        }
    }
}
