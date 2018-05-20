using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks.Position {

    /// <summary>
    /// High precision position in 3d space that has been broken into 3d grid cells
    /// </summary>
    [System.Serializable]
    public class WorldPosition {

        /// <summary>
        /// Grid cell size
        /// </summary>
        /// <returns></returns>
        public static Vector3 defaultSectorSize = new Vector3(1000, 1000, 1000);

        /// <summary>
        /// Position within cell
        /// </summary>
        /// <returns></returns>
        public Vector3 sectorOffset = new Vector3(0,0,0);
        
        /// <summary>
        /// Cell index in world
        /// </summary>
        /// <returns></returns>
        public Long3 sector = new Long3(0,0,0);

        public static readonly WorldPosition zero = new WorldPosition(0.0f,0.0f,0.0f);

        /// <summary>
        /// Create at 0,0,0
        /// </summary>
        public WorldPosition() {
            this.sectorOffset = new Vector3(0f, 0f, 0f);
            this.sector = new Long3(0, 0, 0);
        }

        /// <summary>
        /// Create at some double precision location
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public WorldPosition(Vector3d pos) : this(pos.x, pos.y, pos.z) {
        }
        /// <summary>
        /// Create at some double precision location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public WorldPosition(double x, double y, double z) : this() {
            long deltaSector;

            deltaSector = (long)(x / defaultSectorSize.x);
            x -= defaultSectorSize.x * deltaSector;
            sector.x += deltaSector;

            deltaSector = (long)(y / defaultSectorSize.y);
            y -= defaultSectorSize.y * deltaSector;
            sector.y += deltaSector;

            deltaSector = (long)(z / defaultSectorSize.z);
            z -= defaultSectorSize.z * deltaSector;
            sector.z += deltaSector;

            this.sectorOffset = new Vector3((float)x, (float)y, (float)z);
        }
       
        /// <summary>
        /// Create at some position
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public WorldPosition(Vector3 pos) : this() {
            this.sectorOffset = pos;
            ForceSectorUpdate();
        }

        /// <summary>
        /// Create at some position in a certain grid cell
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="sector"></param>
        /// <returns></returns>
        public WorldPosition(Vector3 pos, Long3 sector) : this() {
            this.sectorOffset = pos;
            this.sector = sector;
            ForceSectorUpdate();
        }

        /// <summary>
        /// Create at 0,0,0 within a grid cell
        /// </summary>
        /// <param name="sector"></param>
        /// <returns></returns>
        public WorldPosition(Long3 sector) : this() {
            this.sector = sector;
        }

        /// <summary>
        /// Copy another position
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public WorldPosition(WorldPosition other) : this() {
            this.sector = new Long3(other.sector);
            this.sectorOffset = new Vector3(other.sectorOffset.x, other.sectorOffset.y, other.sectorOffset.z);
        }

        /// <summary>
        /// Convert to unity3d vector
        /// </summary>
        /// <returns></returns>
        public Vector3 vector3 {
            get {
                return new Vector3(
                    this.sectorOffset.x + this.sector.x * defaultSectorSize.x,
                    this.sectorOffset.y + this.sector.y * defaultSectorSize.y,
                    this.sectorOffset.z + this.sector.z * defaultSectorSize.z
                );
            }
        }

        /// <summary>
        /// Convert to double precision vector
        /// </summary>
        /// <returns></returns>
        public Vector3d vector3d {
            get {
                return new Vector3d(
                    this.sectorOffset.x + this.sector.x * (double)defaultSectorSize.x,
                    this.sectorOffset.y + this.sector.y * (double)defaultSectorSize.y,
                    this.sectorOffset.z + this.sector.z * (double)defaultSectorSize.z
                );
            }
        }

        /// <summary>
        /// Compute vector mangitude
        /// </summary>
        /// <returns></returns>
        public double magnitude {
            get {
                double x = this.sectorOffset.x + this.sector.x * defaultSectorSize.x;
                double y = this.sectorOffset.y + this.sector.y * defaultSectorSize.y;
                double z = this.sectorOffset.z + this.sector.z * defaultSectorSize.z;
                double mag = System.Math.Sqrt(x * x + y * y + z * z);
                return mag;
            }
        }

        /// <summary>
        /// Compute vector magnitude squared
        /// </summary>
        /// <returns></returns>
        public double sqrMagnitude {
            get {
                double x = this.sectorOffset.x + this.sector.x * defaultSectorSize.x;
                double y = this.sectorOffset.y + this.sector.y * defaultSectorSize.y;
                double z = this.sectorOffset.z + this.sector.z * defaultSectorSize.z;
                double sqrMag = (x * x + y * y + z * z);
                return sqrMag;
            }
        }

        /// <summary>
        /// Create a normalized world position. Same direction, length 1
        /// </summary>
        /// <returns></returns>
        public WorldPosition normalized {
            get {
                double invMag = 1.0 / this.magnitude;

                double x = (this.sectorOffset.x + this.sector.x * defaultSectorSize.x) / invMag;
                double y = (this.sectorOffset.y + this.sector.y * defaultSectorSize.y) / invMag;
                double z = (this.sectorOffset.z + this.sector.z * defaultSectorSize.z) / invMag;

                long lx = (long)(x / defaultSectorSize.x);
                long ly = (long)(y / defaultSectorSize.y);
                long lz = (long)(z / defaultSectorSize.z);

                float fx = (float)(x - lx * defaultSectorSize.x);
                float fy = (float)(y - ly * defaultSectorSize.y);
                float fz = (float)(z - lz * defaultSectorSize.z);

                WorldPosition p = new WorldPosition(this);
                p.sectorOffset = new Vector3(fx, fy, fz);
                p.sector = new Long3(lx, ly, lz);
                return p;
            }
        }

        /// <summary>
        /// Extract only the grid cell 
        /// </summary>
        /// <returns></returns>
        public WorldPosition SectorOnly() {
            return new WorldPosition(this.sector);
        }

        /// <summary>
        /// Extract only the position in the grid cell
        /// </summary>
        /// <returns></returns>
        public WorldPosition OffsetOnly() {
            return new WorldPosition(this.sectorOffset);
        }

        /// <summary>
        /// Distance between two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double Distance(WorldPosition a, WorldPosition b) {
            return (b - a).magnitude;
        }

        /// <summary>
        /// Squared distance between two points
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double SqrDistance(WorldPosition a, WorldPosition b) {
            return (b - a).sqrMagnitude;
        }

        /// <summary>
        /// Hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() {
            return sectorOffset.GetHashCode() ^ sector.GetHashCode();
        }

        /// <summary>
        /// Update grid cell and internal offset
        /// </summary>
        public void ForceSectorUpdate() {
            long deltaSector;
            float nx = 0; float ny = 0; float nz = 0;
            long lx = 0; long ly = 0; long lz = 0;

            deltaSector = (long)(sectorOffset.x / defaultSectorSize.x);
            nx = sectorOffset.x - defaultSectorSize.x * deltaSector;
            lx = sector.x + deltaSector;

            deltaSector = (long)(sectorOffset.y / defaultSectorSize.y);
            ny = sectorOffset.y - defaultSectorSize.y * deltaSector;
            ly = sector.y + deltaSector;

            deltaSector = (long)(sectorOffset.z / defaultSectorSize.z);
            nz = sectorOffset.z - defaultSectorSize.z * deltaSector;
            lz = sector.z + deltaSector;

            sectorOffset = new Vector3(nx, ny, nz);
            sector = new Long3(lx, ly, lz);
        }

        /// <summary>
        /// Check if two another world position occupies the same sector of space 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool SameSector(WorldPosition other){
            return this.sector == other.sector;
        }

        /// <summary>
        /// Add two world positions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static WorldPosition operator +(WorldPosition a, WorldPosition b) {
            WorldPosition p = new WorldPosition(a.sectorOffset + b.sectorOffset, a.sector + b.sector);

            return p;
        }

        /// <summary>
        /// Subtract two world positions
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static WorldPosition operator -(WorldPosition a, WorldPosition b) {
            WorldPosition p = new WorldPosition(a.sectorOffset - b.sectorOffset, a.sector - b.sector);

            return p;
        }

        /// <summary>
        /// Test position equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(System.Object other) {
            if (other == null)
                return false;
            if (other is WorldPosition == false)
                return false;
            return sectorOffset == ((WorldPosition)other).sectorOffset && sector == ((WorldPosition)other).sector;
        }

        /// <summary>
        /// Test position equality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator ==(WorldPosition a, System.Object b) {
            if (((object)a) == null) {
                return b == null;
            }

            return a.Equals(b);
        }

        /// <summary>
        /// Test position inequality
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool operator !=(WorldPosition a, System.Object b) {
            if (((object)a) == null) {
                return b != null;
            }
                
            return !a.Equals(b);
        }

        /// <summary>
        /// Convert to a string reprsentation
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            //{1 : 0.004}, {1 : 12.0}, {1 : 100}
            return string.Format("[{0} : {1}],[{2} : {3}],[{4} : {5}]", this.sector.x.ToString(), this.sectorOffset.x.ToString(), this.sector.y.ToString(), this.sectorOffset.y.ToString(), this.sector.z.ToString(), this.sectorOffset.z.ToString());
        }

    }

}