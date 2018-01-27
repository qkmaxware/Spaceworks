using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    [System.Serializable]
    public class WorldPosition : IEquatable<WorldPosition> {

        public static Vector3 defaultSectorSize = new Vector3(1000, 1000, 1000);

        public Vector3 sectorOffset = new Vector3();
        public Long3 sector = new Long3();

        public static readonly WorldPosition zero = new WorldPosition();

        public WorldPosition() {}
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
        public WorldPosition(float x, float y, float z) : this(){
            this.sectorOffset = new Vector3(x, y, z);
            ForceSectorUpdate();
        }
        public WorldPosition(Vector3 pos) : this() {
            this.sectorOffset = pos;
            ForceSectorUpdate();
        }
        public WorldPosition(Vector3 pos, Long3 sector) : this() {
            this.sectorOffset = pos;
            this.sector = sector;
            ForceSectorUpdate();
        }
        public WorldPosition(Long3 sector) : this() {
            this.sector = sector;
        }
        public WorldPosition(WorldPosition other) : this() {
            this.sector = new Long3(other.sector);
            this.sectorOffset = new Vector3(other.sectorOffset.x, other.sectorOffset.y, other.sectorOffset.z);
        }


        public double magnitude {
            get {
                double x = this.sectorOffset.x + this.sector.x * defaultSectorSize.x;
                double y = this.sectorOffset.y + this.sector.y * defaultSectorSize.y;
                double z = this.sectorOffset.z + this.sector.z * defaultSectorSize.z;
                double mag = System.Math.Sqrt(x * x + y * y + z * z);
                return mag;
            }
        }

        public double sqrMagnitude {
            get {
                double x = this.sectorOffset.x + this.sector.x * defaultSectorSize.x;
                double y = this.sectorOffset.y + this.sector.y * defaultSectorSize.y;
                double z = this.sectorOffset.z + this.sector.z * defaultSectorSize.z;
                double sqrMag = (x * x + y * y + z * z);
                return sqrMag;
            }
        }

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

        public WorldPosition SectorOnly() {
            return new WorldPosition(this.sector);
        }

        public WorldPosition OffsetOnly() {
            return new WorldPosition(this.sectorOffset);
        }

        public static double Distance(WorldPosition a, WorldPosition b) {
            return (b - a).magnitude;
        }

        public static double SqrDistance(WorldPosition a, WorldPosition b) {
            return (b - a).sqrMagnitude;
        }

        public bool Equals(WorldPosition other) {
            return sectorOffset == other.sectorOffset && sector == other.sector;
        }

        public override bool Equals(System.Object other) {
            if (other is WorldPosition == false)
                return false;
            return this.Equals((WorldPosition)other);
        }

        public override int GetHashCode() {
            return sectorOffset.GetHashCode() ^ sector.GetHashCode();
        }

        public void ForceSectorUpdate() {
            long deltaSector;

            deltaSector = (long)(sectorOffset.x / defaultSectorSize.x);
            sectorOffset.x -= defaultSectorSize.x * deltaSector;
            sector.x += deltaSector;

            deltaSector = (long)(sectorOffset.y / defaultSectorSize.y);
            sectorOffset.y -= defaultSectorSize.y * deltaSector;
            sector.y += deltaSector;

            deltaSector = (long)(sectorOffset.z / defaultSectorSize.z);
            sectorOffset.z -= defaultSectorSize.z * deltaSector;
            sector.z += deltaSector;

        }

        public Vector3 ToVector3() {
            return new Vector3(
                this.sectorOffset.x + this.sector.x * defaultSectorSize.x,
                this.sectorOffset.y + this.sector.y * defaultSectorSize.y,
                this.sectorOffset.z + this.sector.z * defaultSectorSize.z
            );
        }

        public static WorldPosition operator +(WorldPosition a, WorldPosition b) {
            WorldPosition p = new WorldPosition(a.sectorOffset + b.sectorOffset, a.sector + b.sector);

            return p;
        }

        public static WorldPosition operator -(WorldPosition a, WorldPosition b) {
            WorldPosition p = new WorldPosition(a.sectorOffset - b.sectorOffset, a.sector - b.sector);

            return p;
        }

        public static bool operator ==(System.Object a, WorldPosition b) {
            return a.Equals(b);
        }

        public static bool operator !=(System.Object a, WorldPosition b) {
            return !a.Equals(b);
        }

        public static bool operator ==(WorldPosition a, System.Object b) {
            return a.Equals(b);
        }

        public static bool operator !=(WorldPosition a, System.Object b) {
            return !a.Equals(b);
        }

        public override string ToString() {
            //{1 : 0.004}, {1 : 12.0}, {1 : 100}
            return string.Format("{{0} : {1}},{{2} : {3}},{{4} : {5}}", this.sector.x.ToString(), this.sectorOffset.x.ToString(), this.sector.y.ToString(), this.sectorOffset.y.ToString(), this.sector.z.ToString(), this.sectorOffset.z.ToString());
        }

    }

}