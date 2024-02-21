using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace COM3D2.UndresssHerVR
{
    internal class UndressProfile
    {

        public UndressProfile()
        { 

        }

        public string ProfileName { get; set; } = "Default";


        public bool Wear      { get; set; } = true;
        public bool Skirt     { get; set; } = true;
        public bool Onepiece  { get; set; } = true;
        public bool Mizugi    { get; set; } = true;
        public bool Bra       { get; set; } = true;
        public bool Panz      { get; set; } = true;
        public bool Stkg      { get; set; } = true;
        public bool Shoes     { get; set; } = true;
        public bool Headset   { get; set; } = false;
        public bool AccHat    { get; set; } = true;
        public bool AccHead   { get; set; } = false;
        public bool AccKubi   { get; set; } = false;
        public bool AccKubiwa { get; set; } = false;
        public bool Glove     { get; set; } = true;
        public bool AccUde    { get; set; } = true;
        public bool AccAshi   { get; set; } = false;
        public bool AccSenaka { get; set; } = true;
        public bool AccShippo { get; set; } = false;
        public bool AccHana   { get; set; } = false;
        public bool Megane    { get; set; } = false;
    }
}
