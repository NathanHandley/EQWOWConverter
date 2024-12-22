using System.Linq;
using LanternExtractor.EQ.Wld.DataTypes;
using LanternExtractor.EQ.Wld.Fragments;

namespace LanternExtractor.EQ.Wld.Exporters
{
    public class BspTreeWriter : TextAssetWriter
    {
        public BspTreeWriter()
        {
            Export.AppendLine(LanternStrings.ExportHeaderTitle + "BSP Tree");
            Export.AppendLine(LanternStrings.ExportHeaderFormat +
                               "Normal nodes: NormalX, NormalY, NormalZ, SplitDistance, LeftNodeId, RightNodeId");
            Export.AppendLine(LanternStrings.ExportHeaderFormat +
                               "Leaf nodes: BSPRegionId, RegionType");
        }

        public override void AddFragmentData(WldFragment data)
        {
            BspTree tree = data as BspTree;

            if (tree == null)
            {
                return;
            }

            foreach (var node in tree.Nodes)
            {
                // Normal node
                if (node.Region == null)
                {
                    Export.Append(node.NormalX.ToString(NumberFormat));
                    Export.Append(",");
                    Export.Append(node.NormalZ.ToString(NumberFormat));
                    Export.Append(",");
                    Export.Append(node.NormalY.ToString(NumberFormat));
                    Export.Append(",");
                    Export.Append(node.SplitDistance.ToString(NumberFormat));
                    Export.Append(",");
                    Export.Append(node.LeftNode.ToString(NumberFormat));
                    Export.Append(",");
                    Export.Append(node.RightNode.ToString(NumberFormat));
                    Export.AppendLine();
                }
                else
                // Leaf node
                {
                    Export.Append(node.RegionId.ToString(NumberFormat));
                    Export.Append(",");

                    string types = string.Empty;

                    if (node.Region.RegionType != null)
                    {
                        foreach (var type in node.Region.RegionType.RegionTypes)
                        {
                            types += type.ToString();
                        
                            if(node.Region.RegionType.RegionTypes.Last() != type)
                            {
                                types += ";";
                            }
                        }
                    }
                    else
                    {
                        types = RegionType.Normal.ToString();
                    }
                    
                    Export.Append(types);

                    if (node.Region.RegionType == null)
                    {
                        Export.AppendLine();
                        continue;
                    }

                    if (node.Region.RegionType.RegionTypes.Contains(RegionType.Zoneline))
                    {
                        ZonelineInfo zoneline = node.Region.RegionType?.Zoneline;

                        if (zoneline != null)
                        {
                            Export.Append(",");
                            Export.Append(zoneline.Type.ToString()); 
                            Export.Append(",");

                            if (zoneline.Type == ZonelineType.Reference)
                            {
                                Export.Append(zoneline.Index);
                            }
                            else
                            {                                
                                Export.Append(zoneline.ZoneIndex);
                                Export.Append(",");
                                Export.Append(zoneline.Position.x);
                                Export.Append(",");
                                Export.Append(zoneline.Position.y);
                                Export.Append(",");
                                Export.Append(zoneline.Position.z);
                                Export.Append(",");
                                Export.Append(zoneline.Heading);
                            }
                        }
                    }
                    
                    Export.AppendLine();
                }
            }
        }
    }
}