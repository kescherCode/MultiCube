//namespace MultiCube
//{
//    class Cube
//    {
//        public static void Print2DProjection(float angX, float angY, float angZ)
//        {
//            foreach (CornerData line in lines)
//            {
//                for (int i = 0; i < 25; i++)
//                {
//                    // Find a point between A and B by following formula p=a+z(b-a) where z
//                    // is a value between 0 and 1.
//                    var point = line.a + (i * 1.0f / 24) * (line.b - line.a);
//                    // Rotates the point relative to all the angles given to the method.
//                    Point3D r = point.RotateX(angX).RotateY(angY).RotateZ(angZ);
//                    // Projects the point into 2d space. Acts as a kind of camera setting.
//                    Point3D q = r.Project(0, 0, 100, 3);
//                    // Setting the cursor to the proper positions
//                    int x = ((int)(q.x + Console.WindowWidth * 2.5) / 5);
//                    int y = ((int)(q.y + Console.WindowHeight * 2.5) / 5);
//                    Console.SetCursorPosition(x, y);

//                    Console.Write('°'); // Max Wichmann suggested this symbol
//                }
//            }
//        }
//    }
//}