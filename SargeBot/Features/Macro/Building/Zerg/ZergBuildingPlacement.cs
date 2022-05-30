using SargeBot.Features.GameInfo;
using SC2APIProtocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SargeBot.Features.Macro.Zerg;
public class ZergBuildingPlacement
{
    private MapService MapService;
    public ZergBuildingPlacement(MapService MapService)
    {
        this.MapService = MapService;
    }

    //PreCondition: You have enough resources for the building && It is next in line to be made
    private Point2D FindPlaceMent()
    {
        //Begin with choosing base to put it in?

        return null;
    }




}
