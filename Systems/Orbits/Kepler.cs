using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace Spaceworks.Orbits.Kepler {

    /// <summary>
    /// Class for utlities for kepler calculations
    /// </summary>
    public class KeplerUtils {

        /// <summary>
        /// Inverse Hyperbolic Cosine
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static double Acosh(double a) {
            if (a < 1)
                return 0;
            return Math.Log(a + Math.Sqrt(a * a - 1.0d));
        }

        /// <summary>
        /// Convert eccentric anomaly to true anomaly
        /// https://en.wikipedia.org/wiki/True_anomaly
        /// </summary>
        /// <param name="eccentricAnomaly"></param>
        /// <param name="eccentricity"></param>
        /// <returns></returns>
        public static double EccentricToTrue(double eccentricAnomaly, double eccentricity) {
            if (eccentricity < 1) {
                double cosE = Math.Cos(eccentricAnomaly);

                double t = Math.Acos(
                    (cosE - eccentricity) /
                    (1 - eccentricity * cosE)
                );

                return t;
            }
            else {
                return Math.Atan2(
                    Math.Sqrt(eccentricity * eccentricity - 1) * Math.Sinh (eccentricAnomaly),
                    eccentricity - Math.Cosh (eccentricAnomaly)
                );
            }
        }

        /// <summary>
        /// Convert true anomaly to eccentric anomaly
        /// </summary>
        /// <param name="trueAnomaly"></param>
        /// <param name="eccentricity"></param>
        /// <returns></returns>
        public static double TrueToEccentric(double trueAnomaly, double eccentricity) {
            if (double.IsNaN(eccentricity) || double.IsInfinity(eccentricity)) {
                return trueAnomaly;
            }

            trueAnomaly = trueAnomaly % KeplerConstants.TWO_PI;

            if (eccentricity < 1) {
                if (trueAnomaly < 0) {
                    trueAnomaly += KeplerConstants.TWO_PI;
                }

                //Inverse of the Eccentric to True
                double cosT = Math.Cos(trueAnomaly);
                double ecc = Math.Acos(
                    (eccentricity + cosT) /
                    (1 + eccentricity * cosT)
                );

                if (trueAnomaly > KeplerConstants.PI) {
                    ecc = KeplerConstants.TWO_PI - ecc;
                }

                return ecc;
            }
            else {
                double cosT = Math.Cos(trueAnomaly);
                double ecc = Acosh(
                    ((eccentricity + cosT) /
                    (1 + eccentricity * cosT)) * Math.Sign(trueAnomaly)
                );
                return ecc;
            }
        }

        /// <summary>
        /// Convert the mean anomaly to the eccentric anomaly
        /// </summary>
        /// <param name="meanAnomaly"></param>
        /// <param name="eccentricity"></param>
        /// <returns></returns>
        public static double MeanToEccentric(double meanAnomaly, double eccentricity) {
            if (eccentricity < 1) {
                return SolveKeplerEquation(meanAnomaly, eccentricity);
            }
            else {
                return SolveHyperbolicKeplerEquation(meanAnomaly, eccentricity);    
            }
        }

        /// <summary>
        /// Convert the eccentric anomaly to the mean anomaly
        /// </summary>
        /// <param name="eccentricAnomaly"></param>
        /// <param name="eccentricity"></param>
        /// <returns></returns>
        public static double EccentricToMean(double eccentricAnomaly, double eccentricity) {
            if (eccentricity < 1) {
                return eccentricAnomaly - eccentricity * Math.Sin(eccentricAnomaly);
            }
            else {
                return Math.Sinh(eccentricAnomaly) * eccentricity - eccentricAnomaly;
            }
        }

        /// <summary>
        /// Iterative approach to solve the kepler equation
        /// </summary>
        /// <param name="meanAnomaly"></param>
        /// <param name="eccentricity"></param>
        /// <returns></returns>
        private static double SolveKeplerEquation(double meanAnomaly, double eccentricity) {
            //Iterative approach
            int iterations = eccentricity < 0.4 ? 2 : 4;
            double e = meanAnomaly;

            for (int i = 0; i < iterations; i++) {
                double esin = eccentricity * Math.Sin(e);
                double ecos = eccentricity * Math.Cos(e);
                double del = e - esin - meanAnomaly;
                double n = 1 - ecos;

                e += -5 * del / (n + Math.Sign(n) * Math.Sqrt(Math.Abs(16 * n * n - 20 * del * esin)));
            }

            return e;
        }

        /// <summary>
        /// Iterative approach to solve the kepler equation for hyperbolic orbits
        /// </summary>
        /// <param name="meanAnomaly"></param>
        /// <param name="eccentricity"></param>
        /// <returns></returns>
        public static double SolveHyperbolicKeplerEquation(double meanAnomaly, double eccentricity) {
            double ep = 1e-005d;
            double del = 1;
            
            double f = Math.Log(2d * System.Math.Abs(meanAnomaly) / eccentricity + 1.8d);
            if (double.IsNaN(f) || double.IsInfinity(f)) {
                return meanAnomaly;
            }

            int maxIteration = 1000; int i = 0;
            while (Math.Abs(del) > ep) {
                del = (eccentricity * (float)System.Math.Sinh(f) - f - meanAnomaly) / (eccentricity * (float)System.Math.Cosh(f) - 1d);
                if (double.IsNaN(del) || double.IsInfinity(del)) {
                    return f;
                }
                
                f -= del;

                if (!(i < ++maxIteration))
                    break;
            }

            return f;
        }

    }

    /// <summary>
    /// Class for storing required parameters to create keplerian orbits
    /// </summary>
    [System.Serializable]
    public class KeplerOrbitalParameters {
        /// <summary>
        /// Length of the semi-major axis 'a'
        /// </summary>
        public double semiMajorLength;

        /// <summary>
        /// Eccentricity describing the shape of the ellipse 'e'
        /// </summary>
        public double eccentricity;

        /// <summary>
        /// Mean anomaly angle corresponding to the body's eccentric anomaly
        /// </summary>
        public double meanAnomaly;

        /// <summary>
        /// Angle of inclination of orbital plane
        /// </summary>
        public double inclination;

        /// <summary>
        /// Angle of argument for the periapsis. Angle from ascending node to periapsis
        /// </summary>
        public double perifocus;

        /// <summary>
        /// Angle of reference to where the orbit passes through the plane of reference 
        /// </summary>
        public double ascendingNode;

        /// <summary>
        /// Create orbital parameters from an object in a cartesian coordinate system centered on a large mass in which to orbit
        /// </summary>
        /// <param name="mu">standard gravitational parameter</param>
        /// <param name="position">relative position</param>
        /// <param name="velocity">velocity</param>
        /// <returns></returns>
        public static KeplerOrbitalParameters FromCartesian(double mu, Vector3d position, Vector3d velocity) {
            KeplerOrbitalParameters p = new KeplerOrbitalParameters();

            //Angular momentum
            Vector3d angularMomentum = Vector3d.Cross(position, velocity);
            Vector3d normal = angularMomentum.normalized;

            double rotation = Math.Atan(angularMomentum.x / (-angularMomentum.y));
            double i = Math.Atan(Math.Sqrt(angularMomentum.x * angularMomentum.x + angularMomentum.y * angularMomentum.y) / angularMomentum.z);

            //Rotate position into orbital frame
            Vector3d ascendingNodeAxis = Vector3d.Rotate(position, rotation, new Vector3d(0, 0, 1));
            ascendingNodeAxis = Vector3d.Rotate(ascendingNodeAxis, i, new Vector3d(1, 0, 0));

            //Determine the latitude
            double lat = Math.Atan(ascendingNodeAxis.y / ascendingNodeAxis.x);

            double distance = position.magnitude;
            double speed = velocity.magnitude;
            double angularMomentumSpeed = angularMomentum.magnitude;

            //Determine semi-major axis and eccentricity
            double a = (mu * distance) / (2 * mu - distance * (speed * speed));
            double e = Math.Sqrt(1 - (angularMomentumSpeed * angularMomentumSpeed / (mu * a)));

            //Radial velocity
            double radSpeed = Vector3d.Dot(position, velocity) / distance;

            double sin_E = (a - distance) / (a * e);
            double cos_E = (distance * radSpeed) / (e * Math.Sqrt(mu * a));

            double v = Math.Atan((Math.Sqrt(1 - e * e) * sin_E) / (cos_E - e));

            double rotationPerifocus = lat - v;

            //Determine mean anomaly
            double E = Math.Asin(sin_E);
            double M = E - e * sin_E;

            //Set values
            p.ascendingNode = rotation;
            p.eccentricity = e;
            p.inclination = i;
            p.meanAnomaly = M;
            p.perifocus = rotationPerifocus;
            p.semiMajorLength = a;

            return p;
        }
    }

    /// <summary>
    /// List of constants used in kepler orbit calculations
    /// </summary>
    public class KeplerConstants {
        //Distance

        public static double KILOMETER_TO_METER = 1000;
        public static double AU_TO_METER = 1.496e+11;
        public static double LIGHT_SECOND_TO_METER = 2.998e+8;
        public static double LIGHT_MINUTE_TO_METER = 1.799e+10;
        public static double LIGHT_HOUR_TO_METER = 1.079e+12;
        public static double LIGHT_DAY_TO_METER = 2.59e+13;
        public static double LIGHT_YEAR_TO_METER = 9.461e+15;

        //Time

        public static int HOURS_TO_SECONDS = 3600;
        public static int DAYS_TO_SECONDS = 86400;
        public static int YEARS_TO_DAYS = DAYS_TO_SECONDS * 365;

        //Rotation

        public static double DEGREE_TO_RADIAN = 0.0174533;
        public static double PI = Math.PI;
        public static double TWO_PI = 2 * PI;

        //Mass

        public static int TONNE_TO_KG = 1000;
        public static double EARTH_MASS = 5.974e24;
        public static double SOLAR_MASS = 1.9891e30;

        //Gravity

        public static double G = 6.674e-11;

    }

    /// <summary>
    /// Class decribing keplerian orbit
    /// </summary>
    public class KeplerOrbit {

        #region base_parameters
        /// <summary>
        /// Body in which this orbit defined around
        /// </summary>
        public KeplerBody parent { get; private set; }

        /// <summary>
        /// Length of the semi-major axis 'a'
        /// </summary>
        public double semiMajorLength { get; private set; }

        /// <summary>
        /// Shortcut for semi-major axis length
        /// </summary>
        public double a {
            get {
                return semiMajorLength;
            }
        }

        /// <summary>
        /// Eccentricity describing the shape of the ellipse 'e'
        /// </summary>
        public double eccentricity { get; private set; }

        /// <summary>
        /// Mean anomaly angle corresponding to the body's eccentric anomaly
        /// </summary>
        public double meanAnomaly { get; private set; }

        /// <summary>
        /// Angle of inclination of orbital plane
        /// </summary>
        public double inclination { get; private set; }

        /// <summary>
        /// Angle of argument for the periapsis. Angle from ascending node to periapsis
        /// </summary>
        public double perifocus { get; private set; }

        /// <summary>
        /// Angle of reference to where the orbit passes through the plane of reference 
        /// </summary>
        public double ascendingNode { get; private set; }

        #endregion

        #region derivable_parameters

        /// <summary>
        /// Length of the semi-minor axis
        /// </summary>
        public double semiMinorAxis {
            get {
                return a * Math.Sqrt(1 - eccentricity * eccentricity);
            }
        }

        /// <summary>
        /// Shortcut for the length of the semi-minor axis
        /// </summary>
        public double b {
            get {
                return semiMinorAxis;
            }
        }

        /// <summary>
        /// Product of G and the mass of the heavier object (m^3/s^2)
        /// </summary>
        public double standardGravitationalParameter {
            get {
                return this.parent.mass * KeplerConstants.G;
            }
        }

        /// <summary>
        /// Length to the periapsis (m)
        /// </summary>
        public double periapsis {
            get {
                //Elliptical
                if (eccentricity < 1) {
                    return (1 - eccentricity) * a;
                }
                //Parabolic
                else if (eccentricity == 1) {
                    return a;
                }
                //Hyperbolic
                else {
                    return (1 - eccentricity) * a;
                }
            }
        }

        /// <summary>
        /// Time to periapsis (s)
        /// </summary>
        public double periapsisTime {
            get {
                //Elliptical
                if (eccentricity < 1) {
                    return Math.Pow(a * a * a / this.standardGravitationalParameter, 0.5) * meanAnomaly;
                }
                //Parabolic
                else if (eccentricity == 1) {
                    return Math.Pow(2 * a * a * a / this.standardGravitationalParameter, 0.5) * meanAnomaly;
                }
                //Hyperbolic
                else {
                    return Math.Pow(-a * a * a / this.standardGravitationalParameter, 0.5) * meanAnomaly;
                }
            }
        }

        /// <summary>
        /// Length to the apoapsis (m)
        /// </summary>
        public double apoapsis {
            get {
                //Elliptical
                if (eccentricity < 1) {
                    return (1 + eccentricity) * a;
                }
                //Hyperbolic or Parabolic
                else {
                    return double.PositiveInfinity;
                }
            }
        }

        /// <summary>
        /// Period of the orbit "sidereal year" (s)
        /// </summary>
        public double period {
            get {
                //Elliptical
                if (eccentricity < 1) {
                    return KeplerConstants.TWO_PI * System.Math.Pow(a * a * a / this.standardGravitationalParameter, 0.5);
                }
                //Parabolic or hyperbolic
                else {
                    return double.PositiveInfinity;
                }
            }
        }

        /// <summary>
        /// Angular speed required for a body to complete one orbit (rad/s)
        /// </summary>
        public double meanAngularSpeed {
            get {
                if (eccentricity < 1) {
                    return Math.Pow(a * a * a / this.standardGravitationalParameter, -0.5);
                }
                else {
                    return double.PositiveInfinity;
                }
            }
        }

        /// <summary>
        /// Shortcut for meanAngularSpeed (rad/s)
        /// </summary>
        public double meanMotion {
            get {
                return this.meanAngularSpeed;
            }
        }

        /// <summary>
        /// The angle of the eccentric anomaly (rad)
        /// </summary>
        public double eccentricAnomaly {
            get {
                return KeplerUtils.MeanToEccentric(this.meanAnomaly, this.eccentricity);
            }
        }

        /// <summary>
        /// The angle of th eTrue anomaly (rad)
        /// </summary>
        public double trueAnomaly {
            get {
                return KeplerUtils.EccentricToTrue(this.eccentricAnomaly, this.eccentricity);
            }
        }

        #endregion

        #region constructors
        /// <summary>
        /// Construct kepler orbit from base parameters
        /// </summary>
        /// <param name="parent">Parent body</param>
        /// <param name="a">semi-major axis length</param> 
        /// <param name="e">eccentricity</param>
        /// <param name="ma">mean anomaly</param>
        /// <param name="i">inclination</param>
        /// <param name="w">perifocus</param>
        /// <param name="omega">ascending node</param>
        public KeplerOrbit(KeplerBody parent, double a, double e, double ma, double i, double w, double omega) {
            this.parent = parent;
            this.semiMajorLength = a;
            this.eccentricity = e;
            this.meanAnomaly = ma;
            this.inclination = i;
            this.perifocus = w;
            this.ascendingNode = omega;
        }

        /// <summary>
        /// Construct kepler orbit from orbital parameters object
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="parameters"></param>
        public KeplerOrbit(KeplerBody parent, KeplerOrbitalParameters parameters) : this(
            parent,
            parameters.semiMajorLength,
            parameters.eccentricity,
            parameters.meanAnomaly,
            parameters.inclination,
            parameters.perifocus,
            parameters.ascendingNode
        ) {}

        #endregion

        #region helper_functions

        /// <summary>
        /// Rotate a vector from the orbital XZ plane to world XYZ 
        /// </summary>
        /// <param name="vec"></param>
        /// <returns></returns>
        private Vector3d undoRotation(Vector3d vec) {

            //Rotate so that the periapsis lines up with the reference vector
            vec = Vector3d.Rotate(vec, -this.perifocus, new Vector3d(0, 0, 1));

            //Rotate so that the orbital plane lines up with the reference plane
            vec = Vector3d.Rotate(vec, -this.inclination, new Vector3d(1,0,0));

            //Rotate so that the ascending node lines up with the reference vector 
            vec = Vector3d.Rotate(vec, -this.ascendingNode, new Vector3d(0, 0, 1));

            return vec;
        }

        #endregion

        #region manipulators

        /// <summary>
        /// Get the coordinate of the object in "local" Cartesian coordinates
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public Vector3d GetCartesianPosition(double angle) {
            //Initial ellipse
            Vector3d pos = new Vector3d(
                a * Math.Cos(angle) - a * eccentricity,
                0,
                b * Math.Sin(angle)
            );
            
            //Transform
            Vector3d finalPos = undoRotation(pos);

            return finalPos;
        }

        /// <summary>
        /// Get the coordinate of the object in "local" Cartesian coordinates (ignore parent position)
        /// </summary>
        /// <returns></returns>
        public Vector3d GetCartesianPosition() {
            return this.GetCartesianPosition(this.eccentricAnomaly);
        }

        /// <summary>
        /// Get the velocity of the object in "local" Cartesian coordinates (ignore parent velocity)
        /// </summary>
        /// <returns></returns>
        public Vector3d GetCartesianVelocity() {
            double e = this.eccentricAnomaly;
            double mm = this.meanMotion;

            Vector3d vel = new Vector3d(
                ((mm * a) / (1 - (eccentricity * Math.Cos(e)))) * (-Math.Sin(e)),
                0,
                ((mm * a) / (1 - (eccentricity * Math.Cos(e)))) * (Math.Sqrt(1 - (eccentricity * eccentricity)) * Math.Cos(e))
            );

            //Transform
            Vector3d finalVel = undoRotation(vel);

            return finalVel;
        }

        /// <summary>
        /// Add velocity to modify the orbit
        /// </summary>
        /// <param name="deltaV"></param>
        public void AddCartesianVelocity(Vector3d deltaV) {
            Vector3d vel = this.GetCartesianVelocity() + deltaV;

            KeplerOrbitalParameters kp = KeplerOrbitalParameters.FromCartesian(this.standardGravitationalParameter, this.GetCartesianPosition(), vel);

            this.semiMajorLength = kp.semiMajorLength;
            this.eccentricity = kp.eccentricity;
            this.meanAnomaly = kp.meanAnomaly;
            this.inclination = kp.inclination;
            this.perifocus = kp.perifocus;
            this.ascendingNode = kp.ascendingNode;
        }

        /// <summary>
        /// Move the object forward or backwards in time by a fixed time-step
        /// </summary>
        public void StepTime(double deltaTime) {
            double mm = this.meanMotion;
            double deltaM = (deltaTime * mm) % (KeplerConstants.TWO_PI);
            double n_ma = (meanAnomaly + deltaM) % (KeplerConstants.TWO_PI);

            this.meanAnomaly = n_ma;
        }

        #endregion

    }

    /// <summary>
    /// Class used to describe a body of a given mass in a specific orbit
    /// </summary>
    public class KeplerBody {

        /// <summary>
        /// Mass of the body in KG
        /// </summary>
        public double mass {
            get; private set;
        }

        /// <summary>
        /// KeplerOrbit object representing the orbital path of this body
        /// </summary>
        public KeplerOrbit orbit {
            get; private set;
        }

        public KeplerBody(double mass, KeplerOrbit orbit){
            this.mass = mass;
            this.orbit = orbit;
        }

    }

}
