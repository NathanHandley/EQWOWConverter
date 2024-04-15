#ifndef __EQCLIENT_WLD_H_
#define __EQCLIENT_WLD_H_

#include "s3d.h"
#include "Types.h"

#define FRAGMENT(name) status = name(&frag_obj, obj, (wld + pos + sizeof(struct_wld_basic_frag)), frag->size, frag->nameRef)
#define FRAGMENT_FUNC(name) int name(void **obj, wld_object *wld, uchar *buf, int len, int frag_name)

typedef struct struct_wld_header {
  int32 magic;
  int32 version;
  int32 fragmentCount;
  int32 header3;
  int32 header4;
  int32 stringHashSize;
  int32 header6;
} struct_wld_header;

typedef struct struct_wld_basic_frag {
  int32 size;
  int32 id;
  int32 nameRef;
} struct_wld_basic_frag;


typedef struct struct_Data36 {
  int32 flags;
  int32 fragment1;
  int32 fragment2;
  int32 fragment3;
  int32 fragment4;
  float centerX;
  float centerY;
  float centerZ;
  int32 params2[3]; // 48
  float maxDist;
  float minX;
  float minY;
  float minZ;
  float maxX;
  float maxY;
  float maxZ; // 24
  short int vertexCount;
  short int texCoordsCount;
  short int normalsCount;
  short int colorCount;
  short int polygonsCount;
  short int size6;
  short int polygonTexCount;
  short int vertexTexCount;
  short int size9;
  short int scale; // 20
} struct_Data36;

struct Data10 {
  int32 flags, size1, fragment;
};

typedef struct Vertex {
  double x, y, z;
  float u, v;
  short skin;
} Vertex;

typedef struct Vert {
  signed short int x, y, z;
} Vert;

typedef struct TexCoordsNew {
  int32 tx, tz;
} TexCoordsNew;

typedef struct TexCoordsOld {
  signed short int tx, tz;
} TexCoordsOld;

typedef struct VertexNormal {
  signed char nx, ny, nz;
} VertexNormal;

typedef struct VertexColor {
  char color[4];
} VertexColor;

typedef struct Polygon {
  short int flags, v1, v2, v3;
} Polygon;

typedef struct Poly {
  int32 flags, v1, v2, v3, tex;
} Poly;

typedef struct Texture {
  int count;
  int *flags;
  int params;
  char **filenames;
  uint32 *textures;
} Texture;

typedef struct Mesh {
  int32 name;
  int32 polygonCount;
  int32 vertexCount;
  Vertex *verti;
  Poly *poly;
  Texture *tex;
} Mesh;

typedef struct ZoneMesh {
  int32 name;
  int32 polygonCount;
  int32 vertexCount;
  Vertex **verti;
  Poly **poly;
  Texture *tex;
} ZoneMesh;

typedef struct struct_frag {
  int32 type;
  int32 name;
  void *frag;
}struct_frag;

typedef struct struct_Data21 {
  float normal[3], splitdistance;
  int32 region, node[2];
} struct_Data21;

typedef struct struct_Data22 {
  int32 flags, fragment1, size1, size2, params1, size3, size4, params2, size5, size6;
} struct_Data22;

/*typedef struct BSP_Region {

} BSP_Region;*/

typedef struct BSP_Node {
  float normal[3], splitdistance;
  //BSP_Region *region;
  struct BSP_Node *left, *right;
} BSP_Node;

typedef struct struct_Data15 {
  uint32 ref, flags, fragment1;
  float trans[3], rot[3];
  float scale[3];
  uint32 fragment2, flags2;
} struct_Data15;

typedef struct Placeable {
  float trans[3], rot[3], scale[3];
  Mesh *mesh;
} Placeable;

typedef struct Placeable_LL {
  Placeable *obj;
  struct Placeable_LL *next;
} Placeable_LL;

typedef struct wld_object {
  int fragCount;
  char loadBSP;
  uchar *wld;
  uchar *sHash;
  uint8 _new;
  s3d_object *s3d;
  Placeable_LL *placeable, *placeable_cur;
  struct wld_object *objs;
  struct_frag **frags;
} wld_object;

#ifdef __cplusplus
extern "C" {
#endif

int WLD_Init(wld_object *obj, uchar *wld, s3d_object *s3d, char loadBSP);
int WLD_GetZoneMesh(wld_object *obj, ZoneMesh *mesh);
#ifdef __cplusplus
}
#endif


#endif
