# Artwork generation notes

The sprites were generated with the built-in imagegen tool using the user's cat reference, then refined using the generated atlas. Transparent backgrounds were requested for every asset.

## Main atlas

Create a 4-column by 3-row sprite atlas of the reference cat with an oversized head, taupe bob hair, blue eyes, a small white ornament in front of the ears, a side black ribbon, and a dark burgundy-and-gold skirt on the quadruped torso and back. The dark shape behind the head is the body's skirt, not a hat.

The first row contains sleeping, hissing, sitting, and blinking poses. The remaining eight cells form a right-facing walk cycle with alternating paws, consistent head and body scale, and a stable ground line. Preserve the costume and camera across frames. Use a clean, transparent background with no labels, shadows, or colored speckles.

## Tail and muzzle refinement

In the hissing cell, curve the raised fluffy tail inward and downward, leaving at least 10 percent of the cell height above the tip. Keep the complete tail visible without shrinking the head.

In all eight walk frames, gently shorten and round the pointed muzzle. Use a soft cheek and a smooth chin transition, preserving the eyes, face identity, pose order, and enlarged head.

## Wake-up stretch

Create six sequential frames in a 3-column by 2-row atlas: wake while lying down, extend the front paws, raise the hindquarters into a deep stretch, lift the head and yawn, pull the paws under the chest, and sit facing the viewer. Match the character, head proportions, colors, costume, and transparency of the main atlas.

## Background cleanup

Remove stray colored pixels, haze, and ground shadows outside each character silhouette. Preserve pale paws, ear ornaments, costume details, pose differences, and the equal-cell atlas layout.
