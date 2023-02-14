using Microsoft.Xna.Framework;
using Nez;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Roguelike.World
{
    public class LevelGenerator
    {
        const string TILED_MAP_EXTENSION = "*.tmx";
        public static Level GenerateLevel(string roomLayoutsDirectory, Dictionary<RoomType, int> roomAmounts)
        {
            var mapsByType = _getMapsByRoomType(roomLayoutsDirectory);
            var rooms = _generateRooms(mapsByType, roomAmounts);
            var level = new Level(rooms);
            return level;
        }
        #region File Handling
        static Dictionary<RoomType, List<string>> _getMapsByRoomType(string roomLayoutsDirectory)
        {
            var subdirs = Directory.GetDirectories(roomLayoutsDirectory);
            Dictionary<RoomType, List<string>> mapsByType = new();
            foreach (var type in Enum.GetValues<RoomType>())
                mapsByType[type] = new List<string>();
            var roomTypesByName = _roomTypesByName();
            foreach (var subdir in subdirs)
            {
                foreach (var typeName in roomTypesByName.Keys)
                {
                    var subdirName = new DirectoryInfo(subdir).Name;
                    if (
                        subdirName.StartsWith(typeName) ||
                        subdirName.StartsWith(typeName.ToLower()) ||
                        subdirName.StartsWith(typeName.ToUpper())
                    )
                    {
                        var type = roomTypesByName[typeName];
                        var mapsInSubdirectory = Directory.GetFiles(subdir, TILED_MAP_EXTENSION);
                        mapsByType[type].AddRange(mapsInSubdirectory);
                        break;
                    }
                }
            }
            return mapsByType;
        }
        #endregion
        # region Random Room Generation
        static Dictionary<Point, Room> _generateRooms(Dictionary<RoomType, List<string>> roomsByType, Dictionary<RoomType, int> roomAmounts)
        {
            var position = Point.Zero;
            Dictionary<Point, Room> rooms = new Dictionary<Point, Room>();
            var remainingTypes = Enum.GetValues<RoomType>().ToList();
            remainingTypes.Remove(RoomType.Boss);

            try
            {
                _makeRoomAndRemoveSection(position, RoomType.Start, rooms, roomsByType, roomAmounts);
                position = _makeAllRoomsOfTypes(
                    position, new RoomType[] { RoomType.Trap, RoomType.Fight }, rooms, roomsByType, roomAmounts
                );
                position = _makeAllRoomsOfTypes(
                    position, remainingTypes, rooms, roomsByType, roomAmounts
                );
                position = _makeAllRoomsOfTypes(
                    position, new RoomType[] { RoomType.Boss}, rooms, roomsByType, roomAmounts
                );
            }
            catch(ArgumentOutOfRangeException ex)
            {
                Debug.Error($"Less tilemaps received than expected. {ex}.");
                throw ex;
            }
            return rooms;
        }
        static void _makeRoomAndRemoveSection(
            Point position,
            RoomType roomType,
            Dictionary<Point, Room> rooms,
            Dictionary<RoomType, List<string>> roomsByType,
            Dictionary<RoomType, int> roomAmounts
        )
        {
            var roomsList = roomsByType[roomType];
            var mapPath = roomsList[RNG.RoomRng.Range(0, roomsList.Count)];
            var room = new Room(mapPath, roomType);
            roomsByType.Remove(roomType);
            roomAmounts.Remove(roomType);
            rooms.Add(position, room);
        }
        static void _makeRoom(
            Point position,
            RoomType roomType,
            Dictionary<Point, Room> rooms,
            Dictionary<RoomType, List<string>> roomsByType
        )
        {
            var roomsList = roomsByType[roomType];
            var mapPath = roomsList[RNG.RoomRng.Range(0, roomsList.Count)];
            var room = new Room(mapPath, roomType);
            roomsList.Remove(mapPath);
            rooms.Add(position, room);
        }
        static Point _makeAllRoomsOfTypes(
            Point startPosition,
            IList<RoomType> roomTypes,
            Dictionary<Point, Room> rooms,
            Dictionary<RoomType, List<string>> roomsByType,
            Dictionary<RoomType, int> roomAmounts
        )
        {
            var position = startPosition;
            var typesToMake = new List<RoomType>();
            foreach(var roomType in roomAmounts.Keys)
                if(roomTypes.Contains(roomType))
                    for (int i = 0; i < roomAmounts.GetValueOrDefault(roomType, 0); i++)
                        typesToMake.Add(roomType);
            typesToMake.Shuffle(RNG.RoomRng);
            foreach (var roomType in typesToMake)
            {
                position = _getEmptySpace(position, rooms);
                _makeRoom(position, roomType, rooms, roomsByType);
            }
            foreach(var roomType in roomTypes)
            {
                roomAmounts.Remove(roomType);
                roomsByType.Remove(roomType);
            }
            return position;
        }
        static Point _getEmptySpace(Point start, Dictionary<Point, Room> rooms)
        {
            Point position = new Point(start.X, start.Y);
            while (rooms.ContainsKey(position))
            {
                position += _randomDirection;
            }
            return position;
        }
        # endregion
        #region Helpers
        static Dictionary<string, RoomType> _roomTypesByName()
        {
            var names = Enum.GetNames(typeof(RoomType));
            var typesByName = new Dictionary<string, RoomType>();
            foreach(var name in names)
            {
                typesByName.Add(name, Enum.Parse<RoomType>(name));
            }
            return typesByName;
        }
        static Point _randomDirection
        {
            get
            {
                var directions = new Point[] { PointExt.Up, PointExt.Right, PointExt.Down, PointExt.Left };
                return directions[RNG.RoomRng.Range(0, directions.Length)];
            }
        }
        #endregion
    }
}
