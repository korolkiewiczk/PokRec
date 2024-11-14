﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Common;

namespace emu
{
    class Program
    {
        private static readonly log4net.ILog log =
            log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(params string[] args)
        {
            try
            {
                var specFilePath = args[0];

                var spec = ImgReconSpec.Load(specFilePath);
                
                if (spec == null) return;

                var mainImg = Bitmap.FromFile(spec.ImgPath);

                ImgReconOutput output = new ImgReconOutput();

                output.SpecResults = new List<ImgReconOutput.Recon>();

                ImgReconOutput prevOutput =
                    spec.LastOutputPath != null ? ImgReconOutput.Load(spec.LastOutputPath) : null;

                foreach (var regionSpec in spec.RegionSpecs)
                {
                    log.Info(regionSpec.Name);
                    var regionMatcher = new RegionMatcher(spec.RegionPath, regionSpec.Name,
                        mainImg,
                        regionSpec.ClassesPath, regionSpec.Num, regionSpec.Threshold, regionSpec.AbandonThreshold,
                        prevOutput != null
                            ? prevOutput.SpecResults.FirstOrDefault(x => x.Name == regionSpec.Name).Values
                            : null);

                    output.SpecResults.Add(new ImgReconOutput.Recon
                    {
                        Name = regionSpec.Name,
                        Values = regionMatcher.Process()
                    });
                }

                output.Save(spec.OutFilePath);               
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}