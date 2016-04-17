if ($Pref::Server::NewBrickTool::PlayerRange $= "")
    $Pref::Server::NewBrickTool::PlayerRange = 250;

if ($Pref::Server::NewBrickTool::AdminRange $= "")
    $Pref::Server::NewBrickTool::AdminRange = 1000;

if ($Pref::Server::NewBrickTool::AllowRepeat $= "")
    $Pref::Server::NewBrickTool::AllowRepeat = true;

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
    uiName = " ";
    doFalloff = "1";
    doDetail = "1";
};

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

    brickImage.onFire(%this, 0, true);
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

    %size = 0.05 + %alpha * 0.2;
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

package NewBrickTool
{
    function brickImage::onFire(%this, %obj, %slot, %repeat)
    {
        if (%obj.client.isAdmin)
            %range = $Pref::Server::NewBrickTool::AdminRange;
        else
            %range = $Pref::Server::NewBrickTool::PlayerRange;

        %origin = %obj.getEyePoint();
        %vector = vectorScale(%obj.getEyeVector(), %range);

        %ray = containerRayCast(%origin, vectorAdd(%origin, %vector),
            $TypeMasks::FxBrickObjectType |
            $TypeMasks::TerrainObjectType |
            $TypeMasks::StaticShapeObjectType
        );

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
                    client = %obj.client;
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
                client = %obj.client;
            };

            MissionCleanup.add(%shape);
        }

        %shape.repeat = %repeat;

        brickDeployProjectile.onCollision(%shape, firstWord(%ray), 1,
            getWords(%ray, 1, 3), getWords(%ray, 4, 6));

        %a = %obj.getMuzzlePoint(0);

        if (isObject(%obj.tempBrick))
            %b = %obj.tempBrick.getWorldBoxCenter();
        else
            %b = getWords(%ray, 1, 3);

        %color = getWords(getColorIDTable(%obj.client.currentColor), 0, 2);
        %shape.newBrickToolFade(%a, %b, %color, 1);
    }

    function brickDeployProjectile::onCollision(%this, %obj, %col, %fade, %pos, %normal, %unk)
    {
        Parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %unk);
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

            if (%state && $Pref::Server::NewBrickTool::AllowRepeat)
                %obj.brickImageRepeat = %obj.schedule(250, "brickImageRepeat");

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
