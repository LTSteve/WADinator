using System;
using UnityEngine;
using WADinator.Structures.Textmap;

public abstract class WADHooks {

    public virtual void CreateThing(Thing thing, GameObject creation) { }

    public virtual void CreateLineDef(LineDef line, GameObject creation) { }

    public virtual void CreateSector(Sector sector, GameObject floor, GameObject ceil) { }

    public virtual void CreateSideDef(SideDef side) { }

    public virtual void CreateVertex(Vertex vert) { }


    public virtual void CreateBlock(BlockDef block) { }
}