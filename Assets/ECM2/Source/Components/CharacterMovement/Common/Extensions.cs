using System;
using UnityEngine;

namespace EasyCharacterMovement
{
    public static class Extensions
    {
        /// <summary>
        /// Return the square of the given value.
        /// </summary>

        public static int square(this int value)
        {
            return value * value;
        }

        /// <summary>
        /// Return the square of the given value.
        /// </summary>

        public static float square(this float value)
        {
            return value * value;
        }

        /// <summary>
        /// Checks whether value is near to zero within a tolerance.
        /// </summary>

        public static bool isZero(this float value)
        {
            const float kTolerance = 0.0000000001f;

            return Mathf.Abs(value) < kTolerance;
        }

        /// <summary>
        /// Returns a copy of given vector with only X component of the vector.
        /// </summary>

        public static Vector3 onlyX(this Vector3 vector3)
        {
            vector3.y = 0.0f;
            vector3.z = 0.0f;

            return vector3;
        }

        /// <summary>
        /// Returns a copy of given vector with only Y component of the vector.
        /// </summary>

        public static Vector3 onlyY(this Vector3 vector3)
        {
            vector3.x = 0.0f;
            vector3.z = 0.0f;

            return vector3;
        }

        /// <summary>
        /// Returns a copy of given vector with only Z component of the vector.
        /// </summary>

        public static Vector3 onlyZ(this Vector3 vector3)
        {
            vector3.x = 0.0f;
            vector3.y = 0.0f;

            return vector3;
        }

        /// <summary>
        /// Returns a copy of given vector with only X and Z components of the vector.
        /// </summary>

        public static Vector3 onlyXZ(this Vector3 vector3)
        {
            vector3.y = 0.0f;

            return vector3;
        }

        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>

        public static bool isZero(this Vector2 vector2)
        {
            return vector2.sqrMagnitude < 9.99999943962493E-11;
        }

        /// <summary>
        /// Checks whether vector is near to zero within a tolerance.
        /// </summary>

        public static bool isZero(this Vector3 vector3)
        {
            return vector3.sqrMagnitude < 9.99999943962493E-11;
        }

        /// <summary>
        /// Checks whether vector is exceeding the magnitude within a small error tolerance.
        /// </summary>

        public static bool isExceeding(this Vector3 vector3, float magnitude)
        {
            // Allow 1% error tolerance, to account for numeric imprecision.

            const float kErrorTolerance = 1.01f;

            return vector3.sqrMagnitude > magnitude * magnitude * kErrorTolerance;
        }

        /// <summary>
        /// Returns a copy of given vector with a magnitude of 1,
        /// and outs its magnitude before normalization.
        /// 
        /// If the vector is too small to be normalized a zero vector will be returned.
        /// </summary>

        public static Vector3 normalized(this Vector3 vector3, out float magnitude)
        {
            magnitude = vector3.magnitude;
            if (magnitude > 9.99999974737875E-06)
                return vector3 / magnitude;

            magnitude = 0.0f;

            return Vector3.zero;
        }

        /// <summary>
        /// Dot product of two vectors.
        /// </summary>        
        
        public static float dot(this Vector3 vector3, Vector3 otherVector3)
        {
            return Vector3.Dot(vector3, otherVector3);
        }

        /// <summary>
        /// Returns a copy of given vector projected onto normal vector.
        /// </summary>

        public static Vector3 projectedOn(this Vector3 thisVector, Vector3 normal)
        {
            return Vector3.Project(thisVector, normal);
        }

        /// <summary>
        /// Returns a copy of given vector projected onto a plane defined by a normal orthogonal to the plane.
        /// </summary>

        public static Vector3 projectedOnPlane(this Vector3 thisVector, Vector3 planeNormal)
        {
            return Vector3.ProjectOnPlane(thisVector, planeNormal);
        }

        /// <summary>
        /// Returns a copy of given vector with its magnitude clamped to maxLength.
        /// </summary>

        public static Vector3 clampedTo(this Vector3 vector3, float maxLength)
        {
            return Vector3.ClampMagnitude(vector3, maxLength);
        }

        /// <summary>
        /// Returns a copy of given vector perpendicular to other vector.
        /// </summary>

        public static Vector3 perpendicularTo(this Vector3 thisVector, Vector3 otherVector)
        {
            return Vector3.Cross(thisVector, otherVector).normalized;
        }

        /// <summary>
        /// Returns a copy of given vector adjusted to be tangent to a specified surface normal relatively to given up axis.
        /// </summary>

        public static Vector3 tangentTo(this Vector3 thisVector, Vector3 normal, Vector3 up)
        {
            Vector3 r = thisVector.perpendicularTo(up);
            Vector3 t = normal.perpendicularTo(r);

            return t * thisVector.magnitude;
        }

        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by world up axis.
        /// </summary>

        public static Vector3 relativeTo(this Vector3 vector3, Transform relativeToThis, bool isPlanar = true)
        {
            Vector3 forward = relativeToThis.forward;

            if (isPlanar)
            {
                Vector3 upAxis = Vector3.up;
                forward = forward.projectedOnPlane(upAxis);

                if (forward.isZero())
                    forward = Vector3.ProjectOnPlane(relativeToThis.up, upAxis);
            }
            
            Quaternion q = Quaternion.LookRotation(forward);

            return q * vector3;
        }

        /// <summary>
        /// Transforms a vector to be relative to given transform.
        /// If isPlanar == true, the transform will be applied on the plane defined by upAxis.
        /// </summary>

        public static Vector3 relativeTo(this Vector3 vector3, Transform relativeToThis, Vector3 upAxis, bool isPlanar = true)
        {
            Vector3 forward = relativeToThis.forward;

            if (isPlanar)
            {
                forward = Vector3.ProjectOnPlane(forward, upAxis);

                if (forward.isZero())
                    forward = Vector3.ProjectOnPlane(relativeToThis.up, upAxis);
            }

            Quaternion q = Quaternion.LookRotation(forward, upAxis);

            return q * vector3;
        }

        /// <summary>
        /// Clamps the given quaternion pitch rotation between the given minPitchAngle and maxPitchAngle.
        /// </summary>

        public static Quaternion clampPitch(this Quaternion quaternion, float minPitchAngle, float maxPitchAngle)
        {
            quaternion.x /= quaternion.w;
            quaternion.y /= quaternion.w;
            quaternion.z /= quaternion.w;
            quaternion.w = 1.0f;

            float pitch = Mathf.Clamp(2.0f * Mathf.Rad2Deg * Mathf.Atan(quaternion.x), minPitchAngle, maxPitchAngle);

            quaternion.x = Mathf.Tan(pitch * 0.5f * Mathf.Deg2Rad);

            return quaternion;
        }
        
        private const int InsertionSortThreshold = 16;

        /// <summary>
        /// 支持直接使用comparison的sort避免IComparer的GC仅使用了快排和插入排序
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        /// <param name="length"></param>
        /// <param name="comparison"></param>
        /// <typeparam name="T"></typeparam>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static void Sort<T>(this T[] array, int index, int length, Comparison<T> comparison)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));
            if (comparison == null)
                throw new ArgumentNullException(nameof(comparison));
            if (index < 0 || length < 0 || index + length > array.Length)
                throw new ArgumentOutOfRangeException();
      
            if (length > 1)
            {
                QuickSort(array, index, index + length - 1, comparison);
            }
        }
      
        private static void QuickSort<T>(T[] array, int low, int high, Comparison<T> comparison)
        {
            while (low < high)
            {
                // If the partition size is small, use insertion sort
                if (high - low < InsertionSortThreshold)
                {
                    InsertionSort(array, low, high, comparison);
                    break;
                }
                else
                {
                    int pivotIndex = Partition(array, low, high, comparison);
                    // Recursively sort the smaller partition first to minimize stack depth
                    if (pivotIndex - low < high - pivotIndex)
                    {
                        QuickSort(array, low, pivotIndex - 1, comparison);
                        low = pivotIndex + 1;
                    }
                    else
                    {
                        QuickSort(array, pivotIndex + 1, high, comparison);
                        high = pivotIndex - 1;
                    }
                }
            }
        }
      
        private static void InsertionSort<T>(T[] array, int low, int high, Comparison<T> comparison)
        {
            for (int i = low + 1; i <= high; i++)
            {
                T key = array[i];
                int j = i - 1;
                while (j >= low && comparison(array[j], key) > 0)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = key;
            }
        }
      
        private static int Partition<T>(T[] array, int low, int high, Comparison<T> comparison)
        {
            T pivot = array[high];
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (comparison(array[j], pivot) <= 0)
                {
                    i++;
                    Swap(array, i, j);
                }
            }
            Swap(array, i + 1, high);
            return i + 1;
        }
      
        private static void Swap<T>(T[] array, int i, int j)
        {
            (array[i], array[j]) = (array[j], array[i]);
        }
        
    }
}