using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spaceworks {

    /// <summary>
    /// Reprents a sound and it's spelling
    /// </summary>
    public class Phenome {

        /// <summary>
        /// Name of sound
        /// </summary>
        public string name;
        /// <summary>
        /// Ways to spell the sound
        /// </summary>
        public string[] forms;

        public Phenome(string name, params string[] forms) {
            this.name = name;
            this.forms = forms;
        }

    }

    /// <summary>
    /// Generator to create random names from sounds
    /// </summary>
    public class PhenomicNameGenerator {

        private System.Random r = new System.Random();

        /// <summary>
        /// List of phenomes in the English language
        /// </summary>
        public Phenome[] phenomes = new Phenome[]{
            new Phenome("A", "a"),
            new Phenome("B", "b"),
            new Phenome("C", "c", "k"),
            new Phenome("D", "d"),
            new Phenome("E", "e", "ee", "ea", "y"),
            new Phenome("F", "f", "ph"),
            new Phenome("G", "g"),
            new Phenome("H", "h"),
            new Phenome("I", "i", "y", "ie"),
            new Phenome("J", "j"),
            new Phenome("K", "k", "qu"),
            new Phenome("L", "l"),
            new Phenome("M", "m"),
            new Phenome("N", "n", "kn"),
            new Phenome("O", "o"),
            new Phenome("P", "p"),
            new Phenome("Q", "ku"),
            new Phenome("R", "r", "wr"),
            new Phenome("S", "s", "ce", "ci", "cy"),
            new Phenome("T", "t"),
            new Phenome("U", "u", "ew"),
            new Phenome("V", "v"),
            new Phenome("W", "w"),
            new Phenome("X", "x"),
            new Phenome("Y", "y"),
            new Phenome("Z", "z", "s"),
            new Phenome("OO", "oo", "u"),
            new Phenome("AW", "aw", "au"),
            new Phenome("SH", "sh", "ti", "ci"),
            new Phenome("ER", "ir", "ur")
        };

        /// <summary>
        /// Create a name with a length between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public string Generate(int min, int max) {
            double t = r.NextDouble();
            int k = (int)((1 - t) * min + t * max);
            return Generate(k);
        }

        /// <summary>
        /// Create a name with a given length
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public string Generate(int size) {
            string s = "";
            for (int i = 0; i < size; i++) {
                int j = r.Next(phenomes.Length);
                Phenome ph = phenomes[j];

                int k = r.Next(ph.forms.Length);
                s += ph.forms[k];
            }
            return s;
        }

    }

}