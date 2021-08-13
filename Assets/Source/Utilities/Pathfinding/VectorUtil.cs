using UnityEngine;

namespace UnitMan.Source.Utilities.Pathfinding
{
    public static class VectorUtil
    {
        public static bool VectorApproximately(Vector3 v1, Vector2Int v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }

        public static bool VectorApproximately(Vector2 v1, Vector2Int v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }

        public static bool VectorApproximately(Vector3 v1, Vector3 v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }

        public static bool VectorApproximately(Vector2Int v1, Vector3 v2, float toleranceInclusive) {
            return (Mathf.Abs(v1.x - v2.x) <= toleranceInclusive && Mathf.Abs(v1.y - v2.y) <= toleranceInclusive);
        }

        public static Vector2Int VectorToVector2Int(Vector3 vector) {
            return new Vector2Int(Mathf.RoundToInt(vector.x),
                Mathf.RoundToInt(vector.y));
        }

        public static Vector3 ToVector3(Vector2Int vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }

        public static Vector3 ToVector3(Vector2 vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }

        public static Vector3Int ToVector3Int(Vector2 vector)
        {
            return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
        }

        public static Vector3Int ToVector3Int(Vector2Int vector)
        {
            return new Vector3Int(vector.x, vector.y, 0);
        }

        public static Vector3 Vector2IntToVector3(Vector2Int vector)
        {
            return new Vector3(vector.x, vector.y, 0f);
        }

        public static Vector3Int ToVector3Int(Vector3 vector)
        {
            return new Vector3Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.y), 0);
        }

        public static Vector2Int ToVector2Int(Vector3Int vector)
        {
            return new Vector2Int(vector.x, vector.y);
        }

        public static Vector3 ToVector3(Vector3Int vector)
        {
            return new Vector3(vector.x, vector.y, vector.z);
        }

        public static Vector3 Round(Vector3 vector)
        {
            return new Vector3(Mathf.Round(vector.x), Mathf.Round(vector.y), Mathf.Round(vector.z));
        }
    }
}