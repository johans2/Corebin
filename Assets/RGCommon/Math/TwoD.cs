using UnityEngine;

namespace RGCommon {
    /**
     * Functions for doing calculations on 2D objects.
     */
    public abstract class TwoD {

        public static bool TriangleIsInBounds(Rect bounds, Vector2 p0, Vector2 p1, Vector2 p2) {
            return (
                TriangleIntersectsRectangle(p0, p1, p2, bounds) ||
                TriangleContainsPoint(p0, p1, p2, new Vector2(bounds.xMin, bounds.yMin)) ||
                TriangleContainsPoint(p0, p1, p2, new Vector2(bounds.xMax, bounds.yMin)) ||
                TriangleContainsPoint(p0, p1, p2, new Vector2(bounds.xMin, bounds.yMax)) ||
                TriangleContainsPoint(p0, p1, p2, new Vector2(bounds.xMax, bounds.yMax))
           );
        }

        public static bool TriangleContainsPoint(Vector2 vertex0, Vector2 vertex1, Vector2 vertex2, Vector2 point) {
            return (
                PointsOnSameSide(vertex0, vertex1, vertex2, point) &&
                PointsOnSameSide(vertex1, vertex2, vertex0, point) &&
                PointsOnSameSide(vertex2, vertex0, vertex1, point)
            );
        }

        /**
         * Check whether two points are on the same side of a line.
         */
        public static bool PointsOnSameSide(Vector2 line0, Vector2 line1, Vector2 point0, Vector2 point1) {
            return 0 <= (
                Determinant(line0, line1, point0) *
                Determinant(line0, line1, point1)
            );
        }

        /**
         * Get the barycentric coordinate for point p in the triangle p0, p1, p2.
         */
        public static Vector3 ToBarycentric(
            Vector2 p0, Vector2 p1, Vector2 p2,
            Vector2 p
        ) {
            float x = p.x;
            float y = p.y;
            float x1 = p0.x;
            float y1 = p0.y;
            float x2 = p1.x;
            float y2 = p1.y;
            float x3 = p2.x;
            float y3 = p2.y;
            float a = ((y2 - y3) * (x - x3) + (x3 - x2) * (y - y3)) / ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            float b = ((y3 - y1) * (x - x3) + (x1 - x3) * (y - y3)) / ((y2 - y3) * (x1 - x3) + (x3 - x2) * (y1 - y3));
            float c = 1 - a - b;
            return new Vector3(a, b, c);
        }

        /// Calculates the determinant of the matrix
        ///
        /// | x0 y0 1 |
        /// | x1 y1 1 |
        /// | x2 y2 1 |
        ///
        /// This happens to be twice the signed area of the triangle formed by the three points.
        /// This can be used to determine if the points are clockwise.
        public static float Determinant(
            Vector2 p0,
            Vector2 p1,
            Vector2 p2
        ) {
            return p0.x * p1.y + p1.x * p2.y + p2.x * p0.y - p0.x * p2.y - p1.x * p0.y - p2.x * p1.y;
        }

        public static bool TriangleIntersectsRectangle(Vector2 vertex0, Vector2 vertex1, Vector2 vertex2, Rect rect) {

            // Based on http://pastebin.com/XhgvBzVw and converted from JS to C#

            var l = rect.xMin;
            var r = rect.xMax; 
            var t = rect.yMin;
            var b = rect.yMax;

            var x0 = vertex0.x; 
            var y0 = vertex0.y; 
            var x1 = vertex1.x; 
            var y1 = vertex1.y; 
            var x2 = vertex2.x; 
            var y2 = vertex2.y; 

            float s;

            int b0 = ((x0 > l) ? 1 : 0) | (((y0 > t) ? 1 : 0) << 1) |
                (((x0 > r) ? 1 : 0) << 2) | (((y0 > b) ? 1 : 0) << 3);
            if(b0 == 3) {
                return true;
            }

            int b1 = ((x1 > l) ? 1 : 0) | (((y1 > t) ? 1 : 0) << 1) |
                (((x1 > r) ? 1 : 0) << 2) | (((y1 > b) ? 1 : 0) << 3);
            if(b1 == 3) {
                return true;
            }

            int b2 = ((x2 > l) ? 1 : 0) | (((y2 > t) ? 1 : 0) << 1) |
                (((x2 > r) ? 1 : 0) << 2) | (((y2 > b) ? 1 : 0) << 3);
            if(b2 == 3) {
                return true;
            }

            int i0 = b0 ^ b1;
            if(i0 != 0) {
                float m = (y1 - y0) / (x1 - x0);
                float c = y0 - (m * x0);
                if((i0 & 1) != 0) {
                    s = m * l + c;
                    if(s > t && s < b) {
                        return true;
                    }
                }
                if((i0 & 2) != 0) {
                    s = (t - c) / m;
                    if(s > l && s < r) {
                        return true;
                    }
                }
                if((i0 & 4) != 0) {
                    s = m * r + c;
                    if(s > t && s < b) {
                        return true;
                    }
                }
                if((i0 & 8) != 0) {
                    s = (b - c) / m;
                    if(s > l && s < r) {
                        return true;
                    }
                }
            }

            int i1 = b1 ^ b2;
            if(i1 != 0) {
                float m = (y2 - y1) / (x2 - x1);
                float c = y1 - (m * x1);
                if((i1 & 1) != 0) {
                    s = m * l + c;
                    if(s > t && s < b) {
                        return true;
                    }
                }
                if((i1 & 2) != 0) {
                    s = (t - c) / m;
                    if(s > l && s < r) {
                        return true;
                    }
                }
                if((i1 & 4) != 0) {
                    s = m * r + c;
                    if(s > t && s < b) {
                        return true;
                    }
                }
                if((i1 & 8) != 0) {
                    s = (b - c) / m;
                    if(s > l && s < r) {
                        return true;
                    }
                }
            }

            int i2 = b0 ^ b2;
            if(i2 != 0) {
                float m = (y2 - y0) / (x2 - x0);
                float c = y0 - (m * x0);
                if((i2 & 1) != 0) {
                    s = m * l + c;
                    if(s > t && s < b) {
                        return true;
                    }
                }
                if((i2 & 2) != 0) {
                    s = (t - c) / m;
                    if(s > l && s < r) {
                        return true;
                    }
                }
                if((i2 & 4) != 0) {
                    s = m * r + c;
                    if(s > t && s < b) {
                        return true;
                    }
                }
                if((i2 & 8) != 0) {
                    s = (b - c) / m;
                    if(s > l && s < r) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Get a 2D vector from the X and Z coordinates of a 3D vector.
        /// </summary>
        /// <param name="v">The vector to swizzle</param>
        /// <returns>A 2D vector</returns>
        public static Vector2 SwizzleXZ(Vector3 v) {
            return new Vector2(v.x, v.z);
        }
    }
}
