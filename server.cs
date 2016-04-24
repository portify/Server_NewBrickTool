if ($Pref::Server::NewBrickTool::PlayerRange $= "")
    $Pref::Server::NewBrickTool::PlayerRange = 1000;

if ($Pref::Server::NewBrickTool::AdminRange $= "")
    $Pref::Server::NewBrickTool::AdminRange = 1000;

if ($Pref::Server::NewBrickTool::AllowRepeat $= "")
    $Pref::Server::NewBrickTool::AllowRepeat = true;

if ($Pref::Server::NewBrickTool::AdminOrb $= "")
    $Pref::Server::NewBrickTool::AdminOrb = true;

if ($Pref::Server::NewBrickTool::IgnoreRayCasting $= "")
    $Pref::Server::NewBrickTool::IgnoreRayCasting = true;

datablock ParticleData(brickTrailParticle)
{
   dragCoefficient = "0";
   windCoefficient = "0";
   gravityCoefficient = "0";
   inheritedVelFactor = "0";
   constantAcceleration = "0";
   lifetimeMS = "0";
   lifetimeVarianceMS = "0";
   spinSpeed = "0";
   spinRandomMin = "0";
   spinRandomMax = "0";
   useInvAlpha = "0";
   animateTexture = "0";
   framesPerSec = "1";
   textureName = "base/data/shapes/blank";
   animTexName[0] = "base/data/shapes/blank";
   colors[0] = "0 0 0 0";
   colors[1] = "0 0 0 0";
   colors[2] = "0 0 0 0";
   colors[3] = "0 0 0 0";
   sizes[0] = "0";
   sizes[1] = "0";
   sizes[2] = "0";
   sizes[3] = "0";
   times[0] = "0";
   times[1] = "1";
   times[2] = "1";
   times[3] = "1";
};

datablock ParticleEmitterData(brickTrailEmitter)
{
    className = "ParticleEmitterData";
    ejectionPeriodMS = "5000";
    periodVarianceMS = "0";
    ejectionVelocity = "0";
    velocityVariance = "0";
    ejectionOffset = "0";
    thetaMin = "0";
    thetaMax = "0";
    phiReferenceVel = "0";
    phiVariance = "360";
    overrideAdvance = "0";
    orientParticles = "0";
    orientOnVelocity = "1";
    particles = "brickTrailParticle";
    lifetimeMS = "0";
    lifetimeVarianceMS = "0";
    useEmitterSizes = "0";
    useEmitterColors = "0";
    uiName = "";
    doFalloff = "1";
    doDetail = "1";
};

brickTrailEmitter.uiName = "";

datablock ParticleData(brickTrailOrigParticle)
{
   dragCoefficient = "2.99998";
   windCoefficient = "0";
   gravityCoefficient = "0";
   inheritedVelFactor = "0";
   constantAcceleration = "0";
   lifetimeMS = "250";
   lifetimeVarianceMS = "0";
   spinSpeed = "10";
   spinRandomMin = "-50";
   spinRandomMax = "50";
   useInvAlpha = "0";
   animateTexture = "0";
   framesPerSec = "1";
   textureName = "base/data/particles/dot";
   animTexName[0] = "base/data/particles/dot";
   colors[0] = "0.200000 0.200000 1.000000 0.466667";
   colors[1] = "0.000000 0.000000 1.000000 0.800000";
   colors[2] = "0.200000 0.200000 1.000000 0.000000";
   colors[3] = "1.000000 1.000000 1.000000 1.000000";
   sizes[0] = "0";
   sizes[1] = "0.299091";
   sizes[2] = "0.00915583";
   sizes[3] = "1";
   times[0] = "0";
   times[1] = "0.298039";
   times[2] = "1";
   times[3] = "2";
};

datablock ParticleEmitterData(brickTrailOrigEmitter)
{
    className = "ParticleEmitterData";
    ejectionPeriodMS = "1";
    periodVarianceMS = "0";
    ejectionVelocity = "60";
    velocityVariance = "0";
    ejectionOffset = "0";
    thetaMin = "0";
    thetaMax = "0";
    phiReferenceVel = "0";
    phiVariance = "360";
    overrideAdvance = "0";
    orientParticles = "0";
    orientOnVelocity = "1";
    particles = "brickTrailOrigParticle";
    lifetimeMS = "0";
    lifetimeVarianceMS = "0";
    useEmitterSizes = "0";
    useEmitterColors = "0";
    uiName = "Brick Trail";
    doFalloff = "1";
    doDetail = "1";
};

datablock StaticShapeData(NewBrickToolTrailShape)
{
    shapeFile = "./cylinder_glow.dts";
};

function serverCmdClearGrid(%client)
{
    messageClient(%client, '', '\c5Brick tool grid cleared.');
    %client.player.nbtUseGrid = false;
    return;
}

function serverCmdSetGrid(%client, %x, %y, %z)
{
    %player = %client.player;
    %brick = %player.tempBrick;

    if (!isObject(%brick))
    {
        messageClient(%client, '', '\c5You need to have a ghost brick to set the brick tool grid.');
        return;
    }

    %data = %brick.getDataBlock();

    if (%x $= "" || %x $= "auto")
        %x = %data.brickSizeX;
    else
        %x = getMax(1, %x | 0);

    if (%y $= "" || %y $= "auto")
        %y = %data.brickSizeY;
    else
        %y = getMax(1, %y | 0);

    if (%z $= "" || %z $= "auto")
        %z = %data.brickSizeZ;
    else
        %z = getMax(1, %z | 0);

    %client.player.nbtUseGrid = true;

    %client.player.nbtGridSizeX = 2 / %x;
    %client.player.nbtGridSizeY = 2 / %y;
    %client.player.nbtGridSizeZ = 5 / %z;

    %box = %brick.getPosition();

    %client.player.nbtGridOriginX = getWord(%box, 0);
    %client.player.nbtGridOriginY = getWord(%box, 1);
    %client.player.nbtGridOriginZ = getWord(%box, 2);

    messageClient(%client, '', '\c5Brick tool grid set to %1x%2x%3.', %x, %y, %z);
}

function Player::brickImageRepeat(%this)
{
    if (%this.getState() $= "Dead" || %this.getMountedImage(0) != nameToID("brickImage"))
    {
        %shape = %obj.brickImageRepeatShape;

        if (isObject(%shape))
        {
            %shape.delete();
            %obj.brickImageRepeatShape = "";
        }

        return;
    }

    newBrickToolFire(%this, true);
    %this.brickImageRepeat = %this.schedule(32, "brickImageRepeat");
}

function StaticShape::newBrickToolDisableRepeat(%this)
{
    %this.repeat = "";
    %this.newBrickToolFade(%this.a, %this.b, %this.color, 1);
}

function StaticShape::newBrickToolFade(%this, %a, %b, %color, %alpha)
{
    if (%alpha <= 0)
    {
        %this.delete();
        return;
    }

    %size = 0.05 + %alpha * 0.075;
    %vector = vectorNormalize(vectorSub(%b, %a));

    %xyz = vectorNormalize(vectorCross("1 0 0", %vector));
    %u = mACos(vectorDot("1 0 0", %vector)) * -1;

    %this.setTransform(vectorScale(vectorAdd(%a, %b), 0.5) SPC %xyz SPC %u);
    %this.setScale(vectorDist(%a, %b) SPC %size SPC %size);
    %this.setNodeColor("ALL", %color SPC %alpha);

    if (%this.repeat)
    {
        %this.a = %a;
        %this.b = %b;
        %this.color = %color;
        return;
    }

    %this.schedule(25, "newBrickToolFade", %a, %b, %color, %alpha - 0.1);
}

function placeNewGhostBrick(%client, %pos, %normal, %noOrient)
{
    if ($Game::MissionCleaningUp)
        return;

    %player = %client.player;

    if (!%player)
        return;

    if (%client.currInv > 0)
        %data = %client.inventory[%client.currInv];
    else if (isObject(%client.instantUseData))
        %data = %client.instantUseData;

    if (!isObject(%data))
        return;

    if (%data.getClassName() !$= "fxDTSBrickData")
        return;

    if (!isObject(%player.tempBrick))
    {
        %player.tempBrick = new fxDTSBrick()
        {
            dataBlock = %data;
        };

        if (isObject(%client.brickGroup))
            %client.brickGroup.add(%player.tempBrick);
        else
            error("ERROR: brickDeployProjectile::onCollision() - client \"" @ %client.getPlayerName() @ "\" has no brick group.");

        %player.tempBrick.setColor(%client.currentColor);

        %noOrient = false;
    }
    else
        %player.tempBrick.setDatablock(%data);

    %aspectRatio = %data.printAspectRatio;

    if (%aspectRatio !$= "")
    {
        if (%client.lastPrint[%aspectRatio] !$= "")
            %player.tempBrick.setPrint(%client.lastPrint[%aspectRatio]);
        else
            %player.tempBrick.setPrint($printNameTable["letters/A"]);
    }

    if (!%noOrient)
    {
        %player.tempBrick.angleID = getAngleIDFromPlayer(%player);

        switch ((%player.tempBrick.angleID + %data.orientationFix) % 4)
        {
            case 0: %rot = "0 0 1 0";
            case 1: %rot = "0 0 -1 1.5708";
            case 2: %rot = "0 0 1 3.14159";
            case 3: %rot = "0 0 1 1.5708";
        }
    }
    else
        %rot = getWords(%player.tempBrick.getTransform(), 3, 6);

    %box = %player.tempBrick.getObjectBox();

    if ((%player.tempBrick.angleID) % 2 == 1)
    {
        %sizeX = getWord(%box, 4);
        %sizeY = getWord(%box, 3);

        %brickSizeX = %data.brickSizeY;
        %brickSizeY = %data.brickSizeX;
    }
    else
    {
        %sizeX = getWord(%box, 3);
        %sizeY = getWord(%box, 4);

        %brickSizeX = %data.brickSizeX;
        %brickSizeY = %data.brickSizeY;
    }

    %offX = %sizeX           * mFloatLength(getWord(%normal, 0), 0);
    %offY = %sizeY           * mFloatLength(getWord(%normal, 1), 0);
    %offZ = getWord(%box, 5) * mFloatLength(getWord(%normal, 2), 0);

    %posX = getWord(%pos, 0) + %offX;
    %posY = getWord(%pos, 1) + %offY;
    %posZ = getWord(%pos, 2) + %offZ;

    if (     %brickSizeX % 2 == 1) %posX -= 0.25;
    if (     %brickSizeY % 2 == 1) %posY -= 0.25;
    if (%data.brickSizeZ % 2 == 1) %posZ -= 0.1;

    if (%player.nbtUseGrid)
    {
        %posX -= %player.nbtGridOriginX;
        %posY -= %player.nbtGridOriginY;
        %posZ -= %player.nbtGridOriginZ;

        %posX = mFloatLength(%posX * %player.nbtGridSizeX, 0) / %player.nbtGridSizeX;
        %posY = mFloatLength(%posY * %player.nbtGridSizeY, 0) / %player.nbtGridSizeY;
        %posZ = mFloatLength(%posZ * %player.nbtGridSizeZ, 0) / %player.nbtGridSizeZ;
    }
    else
    {
        %posX = mFloatLength(%posX * 2, 0) / 2;
        %posY = mFloatLength(%posY * 2, 0) / 2;
        %posZ = mFloatLength(%posZ * 5, 0) / 5;
    }

    if (%player.nbtUseGrid)
    {
        %posX += %player.nbtGridOriginX;
        %posY += %player.nbtGridOriginY;
        %posZ += %player.nbtGridOriginZ;
    }

    if (     %brickSizeX % 2 == 1) %posX += 0.25;
    if (     %brickSizeY % 2 == 1) %posY += 0.25;
    if (%data.brickSizeZ % 2 == 1) %posZ += 0.1;

    %transform = %posX SPC %posY SPC %posZ SPC %rot;

    if (%transform !$= %player.tempBrick.lastTransformSet)
    {
        %player.tempBrick.setTransform(%transform);
        %player.tempBrick.lastTransformSet = %transform;
    }

    return vectorSub(%player.tempBrick.getWorldBoxCenter(), %offX SPC %offY SPC %offZ);
}

function newBrickToolFire(%obj, %repeat)
{
    %client = %obj.client;

    if (!isObject(%client))
        return;

    if (%client.isAdmin)
        %range = $Pref::Server::NewBrickTool::AdminRange;
    else
        %range = $Pref::Server::NewBrickTool::PlayerRange;

    %control = %client.getControlObject();

    if (%control.getClassName() $= "Camera" &&
        $Pref::Server::NewBrickTool::AdminOrb &&
        !%control.isOrbitMode() && !%control.getControlObject())
    {
        %origin = %control.getEyePoint();
        %vector = vectorScale(%control.getEyeVector(), %range);
        // %a = vectorSub(%origin, "0 0 1");
        %a = MatrixMulPoint(%control.getEyeTransform(), "0.4 0.5 -0.5");
    }
    else
    {
        %origin = %obj.getEyePoint();
        %vector = vectorScale(%obj.getEyeVector(), %range);
        %a = %obj.getMuzzlePoint(0);
    }

    %mask = $TypeMasks::TerrainObjectType
          | $TypeMasks::StaticShapeObjectType;

    if ($Pref::Server::NewBrickTool::IgnoreRayCasting)
        %mask |= $TypeMasks::FxBrickAlwaysObjectType;
    else
        %mask |= $TypeMasks::FxBrickObjectType;

    %ray = containerRayCast(%origin, vectorAdd(%origin, %vector), %mask);

    if (!%ray)
    {
        %shape = %obj.brickImageRepeatShape;

        if (%shape.repeat)
        {
            %shape.newBrickToolDisableRepeat();
            %obj.brickImageRepeatShape = "";
        }

        return;
    }

    if (%repeat)
    {
        %shape = %obj.brickImageRepeatShape;

        if (!isObject(%shape))
        {
            %shape = new StaticShape()
            {
                datablock = NewBrickToolTrailShape;
            };

            MissionCleanup.add(%shape);
            %obj.brickImageRepeatShape = %shape;
        }
    }
    else
    {
        %shape = new StaticShape()
        {
            datablock = NewBrickToolTrailShape;
        };

        MissionCleanup.add(%shape);
    }

    %shape.repeat = %repeat;

    %b = placeNewGhostBrick(%client,
        getWords(%ray, 1, 3), getWords(%ray, 4, 6), %repeat);

    if (%b $= "")
        %b = getWords(%ray, 1, 3);

    // if (isObject(%obj.tempBrick))
    //     // %b = %obj.tempBrick.getWorldBoxCenter();
    //     %b = %bb;
    // else
    //     %b = getWords(%ray, 1, 3);

    %color = getWords(getColorIDTable(%client.currentColor), 0, 2);
    %shape.newBrickToolFade(%a, %b, %color, 1);
}

package NewBrickTool
{
    function brickImage::onFire(%this, %obj, %slot, %repeat)
    {
    }

    function brickImage::onMount(%this, %obj, %slot)
    {
        Parent::onMount(%this, %obj, %slot);

        %client = %obj.client;

        if (!isObject(%client))
            return;

        %control = %client.getControlObject();

        if (%control.getClassName() $= "Camera" && $Pref::Server::NewBrickTool::AdminOrb)
        {
            %text = "<font:palatino linotype:86>\n<bitmap:base/client/ui/crossHair>";
            commandToClient(%client, 'CenterPrint', %text, 0);
        }
    }

    function brickImage::onUnMount(%this, %obj, %slot)
    {
        Parent::onUnMount(%this, %obj, %slot);

        %shape = %obj.brickImageRepeatShape;

        if (isObject(%shape) && %shape.repeat)
        {
            %shape.newBrickToolDisableRepeat();
            %obj.brickImageRepeatShape = "";
        }

        %client = %obj.client;

        if (!isObject(%client))
            return;

        %control = %client.getControlObject();

        if (%control.getClassName() $= "Camera" && $Pref::Server::NewBrickTool::AdminOrb)
            commandToClient(%client, 'ClearCenterPrint');
    }

    function GameConnection::setControlObject(%this, %object)
    {
        %previous = %this.getControlObject();
        Parent::setControlObject(%this, %object);
        %player = %this.player;

        if ($Pref::Server::NewBrickTool::AdminOrb && isObject(%player) &&
            %player.getMountedImage(0) == nameToID("brickImage"))
        {
            if (%object && %object.getClassName() $= "Camera")
            {
                %text = "<font:palatino linotype:86>\n<lmargin:1><bitmap:base/client/ui/crossHair>";
                commandToClient(%this, 'CenterPrint', %text, 0);
            }
            else if (%previous && %previous.getClassName() $= "Camera")
                commandToClient(%this, 'ClearCenterPrint');
        }
    }

    function Observer::onTrigger(%this, %obj, %slot, %state)
    {
        %client = %obj.getControllingClient();
        %player = %client.player;

        if ($Pref::Server::NewBrickTool::AdminOrb && isObject(%player) &&
            !%obj.isOrbitMode() && !%obj.getControlObject() && %slot == 0 &&
            %player.getMountedImage(0) == nameToID("brickImage"))
        {
            %shape = %player.brickImageRepeatShape;

            if (isObject(%shape))
            {
                %player.brickImageRepeatShape = "";
                %shape.repeat = "";
                %shape.newBrickToolFade(%shape.a, %shape.b, %shape.color, 1);
            }

            cancel(%player.brickImageRepeat);

            if (%state)
            {
                %text = "<font:palatino linotype:86>\n<lmargin:1><bitmap:base/client/ui/crossHair>";
                commandToClient(%client, 'CenterPrint', %text, 0);

                if ($Sim::Time - %player.lastBrickImageFire > 0.25)
                {
                    newBrickToolFire(%player, false);
                    %player.lastBrickImageFire = $Sim::Time;
                }

                if ($Pref::Server::NewBrickTool::AllowRepeat)
                    %player.brickImageRepeat = %player.schedule(250, "brickImageRepeat");
            }

            return;
        }

        Parent::onTrigger(%this, %obj, %slot, %state);
    }

    function Armor::onTrigger(%this, %obj, %slot, %state)
    {
        if (%obj.getMountedImage(0) == nameToID("brickImage") && %slot == 0)
        {
            %shape = %obj.brickImageRepeatShape;

            if (isObject(%shape))
            {
                %obj.brickImageRepeatShape = "";
                %shape.repeat = "";
                %shape.newBrickToolFade(%shape.a, %shape.b, %shape.color, 1);
            }

            cancel(%obj.brickImageRepeat);

            if (%state)
            {
                if ($Sim::Time - %obj.lastBrickImageFire > 0.25)
                {
                    newBrickToolFire(%obj, false);
                    %obj.lastBrickImageFire = $Sim::Time;
                }

                if ($Pref::Server::NewBrickTool::AllowRepeat)
                    %obj.brickImageRepeat = %obj.schedule(250, "brickImageRepeat");
            }

            return;
        }

        Parent::onTrigger(%this, %obj, %slot, %state);
    }

    function Armor::onRemove(%this, %obj)
    {
        if (isObject(%obj.brickImageRepeatShape))
            %obj.brickImageRepeatShape.delete();

        Parent::onRemove(%this, %obj);
    }

    function Armor::onDisabled(%this, %obj, %state)
    {
        %shape = %obj.brickImageRepeatShape;

        if (isObject(%shape) && %shape.repeat)
        {
            %shape.newBrickToolDisableRepeat();
            %obj.brickImageRepeatShape = "";
        }

        Parent::onDisabled(%this, %obj, %state);
    }
};

activatePackage("NewBrickTool");
