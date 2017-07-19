using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.IO;

namespace exsdk {
  public partial class Exporter {

    // -----------------------------------------
    // DumpMesh
    // -----------------------------------------

    void DumpMesh (Mesh _mesh, out GLTF _gltf, out BufferInfo _bufInfo) {
      BufferInfo bufInfo = DumpBufferInfoFromMesh(_mesh);
      GLTF_Mesh gltfMesh = DumpGltfMesh(_mesh);

      // buffers
      GLTF_Buffer gltfBuffer = new GLTF_Buffer();

      gltfBuffer.name = bufInfo.name;
      gltfBuffer.uri = bufInfo.id + ".bin";
      gltfBuffer.byteLength = bufInfo.data.Length;

      int bufViewIdx = 0;
      var bufferViews = new List<GLTF_BufferView>();
      var accessors = new List<GLTF_Accessor>();

      // bufferViews
      foreach ( BufferViewInfo bufView in bufInfo.bufferViews ) {
        GLTF_BufferView gltfBufferView = new GLTF_BufferView();

        gltfBufferView.name = bufView.name;
        gltfBufferView.buffer = 0;
        gltfBufferView.byteOffset = bufView.offset;
        gltfBufferView.byteLength = bufView.length;
        gltfBufferView.byteStride = bufView.stride;
        gltfBufferView.target = (int)bufView.type;

        bufferViews.Add(gltfBufferView);

        // accessors
        foreach ( AccessorInfo acc in bufView.accessors ) {
          GLTF_Accessor gltfAccessor = new GLTF_Accessor();

          gltfAccessor.name = acc.name;
          gltfAccessor.bufferView = bufViewIdx;
          gltfAccessor.byteOffset = acc.offset;
          gltfAccessor.componentType = (int)acc.compType;
          gltfAccessor.count = acc.count;
          gltfAccessor.type = acc.attrType.ToString();
          gltfAccessor.min = acc.min;
          gltfAccessor.max = acc.max;

          accessors.Add(gltfAccessor);
        }

        ++bufViewIdx;
      }

      // GLTF
      GLTF gltf = new GLTF();
      gltf.asset = new GLTF_Asset {
        version = "1.0.0",
        generator = "u3d-exporter"
      };
      gltf.accessors = accessors;
      gltf.bufferViews = bufferViews;
      gltf.buffers = new List<GLTF_Buffer> {gltfBuffer};
      gltf.meshes = new List<GLTF_Mesh> {gltfMesh};

      // assignment
      _gltf = gltf;
      _bufInfo = bufInfo;
    }

    // -----------------------------------------
    // DumpGltfMesh
    // -----------------------------------------

    GLTF_Mesh DumpGltfMesh (Mesh _mesh) {
      GLTF_Mesh result = new GLTF_Mesh();
      Dictionary<string,int> attributes = new Dictionary<string,int>();
      int idx = 0;

      // name
      result.name = _mesh.name;

      // primitives
      result.primitives = new List<GLTF_Primitive>();

      // attributes
      if ( _mesh.vertices.Length > 0 ) {
        attributes.Add("POSITION", idx);
        ++idx;
      }

      if ( _mesh.normals.Length > 0 ) {
        attributes.Add("NORMAL", idx);
        ++idx;
      }

      if ( _mesh.tangents.Length > 0 ) {
        attributes.Add("TANGENT", idx);
        ++idx;
      }

      if ( _mesh.colors.Length > 0 ) {
        attributes.Add("COLOR", idx);
        ++idx;
      }

      if ( _mesh.uv.Length > 0 ) {
        attributes.Add("TEXCOORD_0", idx);
        ++idx;
      }

      if ( _mesh.uv2.Length > 0 ) {
        attributes.Add("TEXCOORD_1", idx);
        ++idx;
      }

      if ( _mesh.uv3.Length > 0 ) {
        attributes.Add("TEXCOORD_2", idx);
        ++idx;
      }

      if ( _mesh.uv4.Length > 0 ) {
        attributes.Add("TEXCOORD_3", idx);
        ++idx;
      }

      if ( _mesh.boneWeights.Length > 0 ) {
        attributes.Add("JOINTS_0", idx);
        ++idx;

        attributes.Add("WEIGHTS_0", idx);
        ++idx;
      }

      // primitives
      if ( _mesh.triangles.Length > 0 ) {
        int cnt = _mesh.subMeshCount;

        for ( int i = 0; i < cnt; ++i ) {
          GLTF_Primitive primitive = new GLTF_Primitive();
          primitive.attributes = attributes;
          primitive.indices = idx;
          ++idx;

          result.primitives.Add(primitive);
        }
      } else {
        GLTF_Primitive primitive = new GLTF_Primitive();
        primitive.attributes = attributes;

        result.primitives.Add(primitive);
      }

      return result;
    }

    // -----------------------------------------
    // DumpBufferInfoFromMesh
    // -----------------------------------------

    BufferInfo DumpBufferInfoFromMesh (Mesh _mesh) {
      string id = Utils.AssetID(_mesh);
      int vertexBytes = 0;
      byte[] vertexData;
      List<byte[]> indexDataList = new List<byte[]>();
      Vector3[] vertices = _mesh.vertices;
      Vector3[] normals = _mesh.normals;
      Vector4[] tangents = _mesh.tangents;
      Color[] colors = _mesh.colors;
      Vector2[] uv = _mesh.uv;
      Vector2[] uv2 = _mesh.uv2;
      Vector2[] uv3 = _mesh.uv3;
      Vector2[] uv4 = _mesh.uv4;
      BoneWeight[] boneWeights = _mesh.boneWeights;
      int offsetNormal = 0;
      int offsetTangent = 0;
      int offsetColor = 0;
      int offsetUV = 0;
      int offsetUV2 = 0;
      int offsetUV3 = 0;
      int offsetUV4 = 0;
      int offsetJoint = 0;
      int offsetWeight = 0;
      int offsetBuffer = 0;
      Vector3 minPos = Vector3.zero;
      Vector3 maxPos = Vector3.zero;

      if ( vertices.Length > 0 ) {
        minPos = maxPos = vertices[0];
        vertexBytes += 12; // float32 * 3
      }

      if ( normals.Length > 0 ) {
        offsetNormal = vertexBytes;
        vertexBytes += 12; // float32 * 3
      }

      if ( tangents.Length > 0 ) {
        offsetTangent = vertexBytes;
        vertexBytes += 16; // float32 * 4
      }

      if ( colors.Length > 0 ) {
        offsetColor = vertexBytes;
        vertexBytes += 16; // float32 * 4
      }

      if ( uv.Length > 0 ) {
        offsetUV = vertexBytes;
        vertexBytes += 8; // float32 * 2
      }

      if ( uv2.Length > 0 ) {
        offsetUV2 = vertexBytes;
        vertexBytes += 8; // float32 * 2
      }

      if ( uv3.Length > 0 ) {
        offsetUV3 = vertexBytes;
        vertexBytes += 8; // float32 * 2
      }

      if ( uv4.Length > 0 ) {
        offsetUV4 = vertexBytes;
        vertexBytes += 8; // float32 * 2
      }

      if ( boneWeights.Length > 0 ) {
        offsetJoint = vertexBytes;
        vertexBytes += 8; // uint16 * 4

        offsetWeight = vertexBytes;
        vertexBytes += 16; // float32 * 4
      }

      // vertexData
      using( MemoryStream stream = new MemoryStream( vertexBytes * _mesh.vertexCount ) ) {
        using ( BinaryWriter writer = new BinaryWriter(stream) ) {
          for ( int i = 0; i < _mesh.vertexCount; ++i ) {
            if ( vertices.Length > 0 ) {
              Vector3 vert = vertices[i];
              // NOTE: convert LH to RH
              vert.z = -vert.z;

              writer.Write(vert.x);
              writer.Write(vert.y);
              writer.Write(vert.z);

              if ( vert.x < minPos.x ) {
                minPos.x = vert.x;
              }
              if ( vert.y < minPos.y ) {
                minPos.y = vert.y;
              }
              if ( vert.z < minPos.z ) {
                minPos.z = vert.z;
              }
              if ( vert.x > maxPos.x ) {
                maxPos.x = vert.x;
              }
              if ( vert.y > maxPos.y ) {
                maxPos.y = vert.y;
              }
              if ( vert.z > maxPos.z ) {
                maxPos.z = vert.z;
              }
            }

            if ( normals.Length > 0 ) {
              // NOTE: convert LH to RH
              writer.Write(normals[i].x);
              writer.Write(normals[i].y);
              writer.Write(-normals[i].z);
            }

            if ( tangents.Length > 0 ) {
              // NOTE: convert LH to RH
              writer.Write(tangents[i].x);
              writer.Write(tangents[i].y);
              writer.Write(-tangents[i].z);
              writer.Write(tangents[i].w);
            }

            if ( colors.Length > 0 ) {
              writer.Write(colors[i].r);
              writer.Write(colors[i].g);
              writer.Write(colors[i].b);
              writer.Write(colors[i].a);
            }

            if ( uv.Length > 0 ) {
              writer.Write(uv[i].x);
              writer.Write(uv[i].y);
            }

            if ( uv2.Length > 0 ) {
              writer.Write(uv2[i].x);
              writer.Write(uv2[i].y);
            }

            if ( uv3.Length > 0 ) {
              writer.Write(uv3[i].x);
              writer.Write(uv3[i].y);
            }

            if ( uv4.Length > 0 ) {
              writer.Write(uv4[i].x);
              writer.Write(uv4[i].y);
            }

            if ( boneWeights.Length > 0 ) {
              writer.Write((ushort)boneWeights[i].boneIndex0);
              writer.Write((ushort)boneWeights[i].boneIndex1);
              writer.Write((ushort)boneWeights[i].boneIndex2);
              writer.Write((ushort)boneWeights[i].boneIndex3);

              writer.Write(boneWeights[i].weight0);
              writer.Write(boneWeights[i].weight1);
              writer.Write(boneWeights[i].weight2);
              writer.Write(boneWeights[i].weight3);
            }
          }
        }
        vertexData = stream.ToArray();
      }

      // indexDataList
      for ( int i = 0; i < _mesh.subMeshCount; ++i ) {
        int[] subTriangles = _mesh.GetTriangles(i);

        if ( subTriangles.Length > 0 ) {
          using( MemoryStream stream = new MemoryStream( 2 * subTriangles.Length ) ) {
            using ( BinaryWriter writer = new BinaryWriter(stream) ) {
              // DISABLE
              // for ( int ii = 0; ii < subTriangles.Length; ++ii ) {
              //   writer.Write((ushort)subTriangles[ii]);
              // }

              // NOTE: convert mesh winding order from CW (Unity3D's) to CCW (most webgl programs)
              for ( int ii = 0; ii < subTriangles.Length/3; ++ii ) {
                writer.Write((ushort)subTriangles[3*ii]);
                writer.Write((ushort)subTriangles[3*ii + 2]);
                writer.Write((ushort)subTriangles[3*ii + 1]);
              }
            }

            indexDataList.Add(stream.ToArray());
          }
        }
      }

      // bufferViews
      List<BufferViewInfo> bufferViews = new List<BufferViewInfo>();

      // vbView
      BufferViewInfo vbView = new BufferViewInfo();
      vbView.name = "vb";
      vbView.offset = 0;
      vbView.length = vertexData.Length;
      vbView.stride = vertexBytes;
      vbView.type = BufferType.VERTEX;
      vbView.accessors = new List<AccessorInfo>();

      if ( vertices.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "position";
        acc.offset = 0;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC3;
        acc.min = new object[3] { minPos.x, minPos.y, minPos.z };
        acc.max = new object[3] { maxPos.x, maxPos.y, maxPos.z };

        vbView.accessors.Add(acc);
      }

      if ( normals.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "normal";
        acc.offset = offsetNormal;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC3;
        // TODO: min, max
        vbView.accessors.Add(acc);
      }

      if ( tangents.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "tangent";
        acc.offset = offsetTangent;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC4;
        // TODO: min, max
        vbView.accessors.Add(acc);
      }

      if ( colors.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "color";
        acc.offset = offsetColor;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC4;
        // TODO: min, max
        vbView.accessors.Add(acc);
      }

      if ( uv.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "uv0";
        acc.offset = offsetUV;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC2;
        // TODO: min, max
        vbView.accessors.Add(acc);
      }

      if ( uv2.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "uv1";
        acc.offset = offsetUV2;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC2;
        // TODO: min, max
        vbView.accessors.Add(acc);
      }

      if ( uv3.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "uv2";
        acc.offset = offsetUV3;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC2;
        // TODO: min, max
        vbView.accessors.Add(acc);
      }

      if ( uv4.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "uv3";
        acc.offset = offsetUV4;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC2;
        // TODO: min, max
        vbView.accessors.Add(acc);
      }

      if ( boneWeights.Length > 0 ) {
        AccessorInfo acc = new AccessorInfo();
        acc.name = "joint";
        acc.offset = offsetJoint;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.UINT16;
        acc.attrType = AttrType.VEC4;
        // TODO: min, max
        vbView.accessors.Add(acc);

        acc = new AccessorInfo();
        acc.name = "weight";
        acc.offset = offsetWeight;
        acc.count = _mesh.vertexCount;
        acc.compType = ComponentType.FLOAT32;
        acc.attrType = AttrType.VEC4;
        // TODO: min, max
        vbView.accessors.Add(acc);
      }

      //
      bufferViews.Add(vbView);
      offsetBuffer += vbView.length;

      // ibView
      for ( int i = 0; i < indexDataList.Count; ++i ) {
        byte[] indexData = indexDataList[i];

        BufferViewInfo ibView = new BufferViewInfo();
        ibView.name = "ib" + i;
        ibView.offset = offsetBuffer;
        ibView.length = indexData.Length;
        ibView.type = BufferType.INDEX;
        ibView.accessors = new List<AccessorInfo>();

        AccessorInfo acc = new AccessorInfo();
        acc.name = "indices" + i;
        acc.offset = 0;
        acc.count = indexData.Length / 2;
        acc.compType = ComponentType.UINT16;
        acc.attrType = AttrType.SCALAR;
        ibView.accessors.Add(acc);

        bufferViews.Add(ibView);
        offsetBuffer += ibView.length;
      }

      // data
      byte[] data = new byte[offsetBuffer];
      int offset = 0;

      System.Buffer.BlockCopy( vertexData, 0, data, offset, vertexData.Length );
      offset += vertexData.Length;

      for ( int i = 0; i < indexDataList.Count; ++i ) {
        System.Buffer.BlockCopy( indexDataList[i], 0, data, offset, indexDataList[i].Length );
        offset += indexDataList[i].Length;
      }

      //
      BufferInfo buffer = new BufferInfo();
      buffer.id = id;
      buffer.name = _mesh.name;
      buffer.data = data;
      buffer.bufferViews = bufferViews;

      return buffer;
    }
  }
}