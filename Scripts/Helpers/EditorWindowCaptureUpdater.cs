﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR.Utilities;

public class EditorWindowCaptureUpdater : MonoBehaviour
{
	[SerializeField]
	private EditorWindowCapture m_EditorWindowCapture;
	[SerializeField]
	private RawImage m_RawImage;
	[SerializeField]
	private Material m_Material;

	void Start()
	{
		if (!m_EditorWindowCapture)
			m_EditorWindowCapture = GetComponent<EditorWindowCapture>();

		if (!m_RawImage)
			m_RawImage = GetComponent<RawImage>();

		if (m_RawImage)
		{
			// Texture comes in flipped, so it's necessary to correct it
			var rect = m_RawImage.uvRect;
			rect.height *= -1f;
			m_RawImage.uvRect = rect;
		}

		if (!m_RawImage && !m_Material)
		{
			var renderer = GetComponent<Renderer>();
			m_Material = U.Material.GetMaterialClone(renderer);
		}

		if (m_Material)
		{
			// Texture comes in flipped, so it's necessary to correct it
			var scale = m_Material.mainTextureScale;
			scale.y *= -1f;
			m_Material.mainTextureScale = scale;
		}
	}

	void OnDestroy()
	{
		U.Object.Destroy(m_Material);
	}

	void LateUpdate()
	{
		// Only capture when we are looking at the view
		var camera = U.Camera.GetMainCamera();
		if (camera)
		{
			Plane plane = new Plane(-transform.forward, transform.position);
			m_EditorWindowCapture.capture = plane.GetSide(camera.transform.position);
		}

		var tex = m_EditorWindowCapture.texture;
		if (m_RawImage && m_RawImage.texture != tex)
			m_RawImage.texture = tex;
		
		if (m_Material && m_Material.mainTexture != tex)
			m_Material.mainTexture = tex;

		var texAspect = (float)tex.width / tex.height;

		var localScale = transform.localScale;
		var aspect = localScale.x / localScale.y;
		localScale.y *= aspect / texAspect;
		transform.localScale = localScale;
	}
}
