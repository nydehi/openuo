#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: BoundingFrustum.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using SharpDX;

namespace OpenUO.DirectX9.Graphics
{
    public sealed class BoundingFrustum
    {
        private Matrix _frustumMatrix;
        private Vector3[] _corners;
        private Plane[] _planes;

        public Plane Bottom
        { 
            get { return _planes[5]; }
        }

        public Plane Far
        { 
            get { return _planes[1]; }
        }

        public Plane Left
        { 
            get { return _planes[2]; } 
        }

        public Plane Near 
        { 
            get { return _planes[0]; } 
        }

        public Plane Right
        { 
            get { return _planes[3]; }
        }

        public Plane Top 
        {
            get { return _planes[4]; } 
        }

        public Matrix FrustumMatrix
        {
            get { return _frustumMatrix; }
            private set
            {
                if (_frustumMatrix != value)
                {

                    _corners = new Vector3[8];
                    _planes = new Plane[6];

                    _planes[2].Normal.X = -value.M14 - value.M11;
                    _planes[2].Normal.Y = -value.M24 - value.M21;
                    _planes[2].Normal.Z = -value.M34 - value.M31;
                    _planes[2].D = -value.M44 - value.M41;

                    _planes[3].Normal.X = -value.M14 + value.M11;
                    _planes[3].Normal.Y = -value.M24 + value.M21;
                    _planes[3].Normal.Z = -value.M34 + value.M31;
                    _planes[3].D = -value.M44 + value.M41;

                    _planes[4].Normal.X = -value.M14 + value.M12;
                    _planes[4].Normal.Y = -value.M24 + value.M22;
                    _planes[4].Normal.Z = -value.M34 + value.M32;
                    _planes[4].D = -value.M44 + value.M42;

                    _planes[5].Normal.X = -value.M14 - value.M12;
                    _planes[5].Normal.Y = -value.M24 - value.M22;
                    _planes[5].Normal.Z = -value.M34 - value.M32;
                    _planes[5].D = -value.M44 - value.M42;

                    _planes[0].Normal.X = -value.M13;
                    _planes[0].Normal.Y = -value.M23;
                    _planes[0].Normal.Z = -value.M33;
                    _planes[0].D = -value.M43;

                    _planes[1].Normal.X = -value.M14 + value.M13;
                    _planes[1].Normal.Y = -value.M24 + value.M23;
                    _planes[1].Normal.Z = -value.M34 + value.M33;
                    _planes[1].D = -value.M44 + value.M43;

                    for (int i = 0; i < 6; i++)
                    {
                        float length = _planes[i].Normal.Length();
                        _planes[i].Normal /= length;
                        _planes[i].D /= length;
                    }

                    Ray ray = ComputeIntersectionLine(Near, Left);

                    _corners[0] = ComputeIntersection(Top, ray);
                    _corners[3] = ComputeIntersection(Bottom, ray);

                    ray = ComputeIntersectionLine(Right, Near);

                    _corners[1] = ComputeIntersection(Top, ray);
                    _corners[2] = ComputeIntersection(Bottom, ray);

                    ray = ComputeIntersectionLine(Left, Far);

                    _corners[4] = ComputeIntersection(Top, ray);
                    _corners[7] = ComputeIntersection(Bottom, ray);

                    ray = ComputeIntersectionLine(Far, Right);

                    _corners[5] = ComputeIntersection(Top, ray);
                    _corners[6] = ComputeIntersection(Bottom, ray);
                }
            }
        }

        public BoundingFrustum(Matrix viewProject)
        {
            FrustumMatrix = viewProject;
        }

        public Ray ComputeIntersectionLine(Plane plane1, Plane plane2)
        {
            Ray ray;
            ray.Direction = Vector3.Cross(plane1.Normal, plane2.Normal);
            float length = ray.Direction.LengthSquared();
            ray.Position = Vector3.Cross((-plane1.D * plane2.Normal) + (plane2.D * plane1.Normal), ray.Direction) / length;
            return ray;
        }

        public Vector3 ComputeIntersection(Plane plane, Ray ray)
        {
            float dot = (-plane.D - Vector3.Dot(plane.Normal, ray.Position)) / Vector3.Dot(plane.Normal, ray.Direction);
            return ray.Position + (ray.Direction * dot);
        }

        public ContainmentType Contains(BoundingBox box)
        {
            bool flag = false;
            Plane[] planes = _planes;

            foreach (Plane plane in planes)
            {
                Plane refPlane = plane;

                switch (box.Intersects(ref refPlane))
                {
                    case PlaneIntersectionType.Front:
                        return ContainmentType.Disjoint;

                    case PlaneIntersectionType.Intersecting:
                        flag = true;
                        break;
                }
            }

            if (!flag)
                return ContainmentType.Contains;

            return ContainmentType.Intersects;
        }

        public ContainmentType Contains(BoundingSphere sphere)
        {
            Vector3 center = sphere.Center;
            float radius = sphere.Radius;
            int intersections = 0;

            Plane[] planes = _planes;

            foreach (Plane plane in planes)
            {
                float dot = (plane.Normal.X * center.X) + (plane.Normal.Y * center.Y) + (plane.Normal.Z * center.Z);
                float dotD = dot + plane.D;

                if (dotD > radius)
                    return ContainmentType.Disjoint;

                if (dotD < -radius)
                    intersections++;
            }

            if (intersections != 6)
                return ContainmentType.Intersects;

            return ContainmentType.Contains;
        }

        public ContainmentType Contains(Vector3 vector)
        {
            Plane[] planes = _planes;

            foreach (Plane plane in planes)
            {
                float dot = (plane.Normal.X * vector.X) + (plane.Normal.Y * vector.Y) + (plane.Normal.Z * vector.Z) + plane.D;

                if (dot > 0.000001f)
                    return ContainmentType.Disjoint;
            }

            return ContainmentType.Contains;
        }
    }
}
